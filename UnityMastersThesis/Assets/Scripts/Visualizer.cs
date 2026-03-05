using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Models;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using NUnit.Framework;
using WFC;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;
using Tile = Domain.Models.Tile;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Visualizer : MonoBehaviour
{
    [SerializeField] private TextAsset _layoutFile;
    [SerializeField] private TextAsset _adjacencyFile;
    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private WfcArgs _wfcArgs;

    [SerializeField, UnityEngine.Range(0f, 1f)] private float _animationSpeed = .5f;

    private State _state;

    [Button("Display Map from layout file")]
    public void DisplayMapFromJson()
    {
        List<Tile> tileLayout =
            JsonConvert.DeserializeObject<List<Tile>>(_layoutFile.text);

        DisplayTiles(tileLayout);
    }

    private void DisplayTiles(List<Tile> tileLayout)
    {
        _tilemap.ClearAllTiles();

        foreach (Tile tile in tileLayout)
        {
            try
            {
                TileBase tileBase = _tiles.First(tileBase => tileBase.name == tile.Name);
                _tilemap.SetTile(new Vector3Int(tile.Position.X, -tile.Position.Y, 0), tileBase);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    [Button("Generate and display WFC map")]
    public void GenerateAndDisplayWfcMap()
    {
        WFC.Args.WfcArgs wfcArgs = _wfcArgs.ToArgs();
        var result = WaveFunctionCollapse.Run(wfcArgs);
        var map = result.Map;
        DisplayTiles(map.Tiles);
    }

    [Button("Step")]
    public void Step()
    {
        InitializeState();

        _state = WaveFunctionCollapse.Step(_state);
        DisplayTiles(_state.GetMap().Tiles);
    }

    private void InitializeState()
    {
        if (_state == null)
        {
            _state = _wfcArgs.ToArgs().ToState();
            Debug.Log("Creating initial state.");
        }
    }

    [Button("Complete")]
    public void Complete()
    {
        InitializeState();

        _state = WaveFunctionCollapse.Complete(_state);
        DisplayTiles(_state.GetMap().Tiles);
    }

    [Button("Reset")]
    public void Reset()
    {
        _state = null;
        _tilemap.ClearAllTiles();
    }

    [Button]
    public void ReadAdjacencyData()
    {
        Dictionary<string, Adjacency> adjacencies =
            JsonConvert.DeserializeObject<Dictionary<string, Adjacency>>(_adjacencyFile.text);
        Debug.Log(adjacencies.Count);

        List<Tile> tileLayout =
            JsonConvert.DeserializeObject<List<Tile>>(_layoutFile.text);
        

        List<Vector> coordinates = tileLayout.Select(tile => tile.Position).ToList();
        HashSet<TileType> tileTypes = tileLayout.Select(tile => new TileType(tile.Name)).ToHashSet();

        List<WFC.Args.AdjacencyRule> adjacencyRules = new List<WFC.Args.AdjacencyRule>();
        
        foreach ((string idFrom, Adjacency adjacency) in adjacencies)
        {
            foreach ((string idTo, int count) in adjacency.UpNeighbors)
            {
                TileType from = new TileType(idFrom);
                TileType to = new TileType(idTo);
                Direction direction = Direction.North;
                adjacencyRules.Add(new WFC.Args.AdjacencyRule(from, to, direction));
            }
            
            foreach ((string idTo, int count) in adjacency.RightNeighbors)
            {
                TileType from = new TileType(idFrom);
                TileType to = new TileType(idTo);
                Direction direction = Direction.East;
                adjacencyRules.Add(new WFC.Args.AdjacencyRule(from, to, direction));
            }
            
            foreach ((string idTo, int count) in adjacency.DownNeighbors)
            {
                TileType from = new TileType(idFrom);
                TileType to = new TileType(idTo);
                Direction direction = Direction.South;
                adjacencyRules.Add(new WFC.Args.AdjacencyRule(from, to, direction));
            }
            
            foreach ((string idTo, int count) in adjacency.LeftNeighbors)
            {
                TileType from = new TileType(idFrom);
                TileType to = new TileType(idTo);
                Direction direction = Direction.West;
                adjacencyRules.Add(new WFC.Args.AdjacencyRule(from, to, direction));
            }
        }
        
        WFC.Args.WfcArgs wfcArgs = new WFC.Args.WfcArgs(coordinates, tileTypes, adjacencyRules);
        
        if (_state == null)
        {
            _state = wfcArgs.ToState();
            Debug.Log("Creating initial state.");
        }
        
        _state = WaveFunctionCollapse.Complete(_state);
        Debug.Log("Map completed.");
        DisplayTiles(_state.GetMap().Tiles);
    }

#if UNITY_EDITOR
    private float _lastStepTime;

    [Button("Start Animation")]
    public void PlayInEditor()
    {
        // Prevent double-subscription
        EditorApplication.update -= UpdateEditorAnimation;
        EditorApplication.update += UpdateEditorAnimation;
        _lastStepTime = (float)EditorApplication.timeSinceStartup;
    }

    [Button("Stop Animation")]
    public void StopEditor()
    {
        EditorApplication.update -= UpdateEditorAnimation;
    }

    private void UpdateEditorAnimation()
    {
        if (_state != null && _state.IsComplete)
        {
            StopEditor();
            Debug.Log("WFC Complete.");
            return;
        }

        float currentTime = (float)EditorApplication.timeSinceStartup;
        if (currentTime - _lastStepTime >= _animationSpeed)
        {
            Step();
            _lastStepTime = currentTime;

            // This ensures the Scene View repaints so you see the tiles change
            EditorUtility.SetDirty(_tilemap);
        }
    }
#endif

    [Serializable]
    public class Adjacency
    {
        public Dictionary<string, int> UpNeighbors { get; set; } = new();
        public Dictionary<string, int> DownNeighbors { get; set; } = new();
        public Dictionary<string, int> LeftNeighbors { get; set; } = new();
        public Dictionary<string, int> RightNeighbors { get; set; } = new();
    }
}