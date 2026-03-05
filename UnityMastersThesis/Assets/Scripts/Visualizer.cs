using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using WFC;
using WFC.Extensions;
using WFC.Models;
using Tile = Domain.Models.Tile;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private TextAsset _layoutFile;
    [SerializeField] private TextAsset _adjacencyFile;
    [SerializeField] private TileBase[] _tiles;
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private WfcArgs _wfcArgs;

    private State _state;
    
    [Button("Display Map from layout file")]
    public void DisplayMapFromJson()
    {
        List<Domain.Models.Tile> tileLayout =
            JsonConvert.DeserializeObject<List<Domain.Models.Tile>>(_layoutFile.text);
        
        DisplayTiles(tileLayout);
    }

    private void DisplayTiles(List<Tile> tileLayout)
    {
        _tilemap.ClearAllTiles();
        
        foreach (Domain.Models.Tile tile in tileLayout)
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
        if (_state == null)
        {
            _state = _wfcArgs.ToArgs().ToState();
            Debug.Log("Creating initial state.");
        }

        _state = WaveFunctionCollapse.Step(_state);
        Debug.Log(_state.IsComplete);
        DisplayTiles(_state.GetMap().Tiles);
    }

    [Button("Complete")]
    public void Complete()
    {
        if (_state == null)
        {
            _state = _wfcArgs.ToArgs().ToState();
        }
        
        _state = WaveFunctionCollapse.Complete(_state);
        DisplayTiles(_state.GetMap().Tiles);
    }

    [Button("Reset")]
    public void Reset()
    {
        _state = null;
        _tilemap.ClearAllTiles();
    }
    
    public void ReadAdjacencyData()
    {
        Dictionary<string, Adjacency> adjacencyData =
            JsonUtility.FromJson<Dictionary<string, Adjacency>>(_adjacencyFile.text);
        Debug.Log(adjacencyData.Count);
    }
    
    [Serializable]
    public class Adjacency
    {
        public Dictionary<string, int> UpNeighbors { get; set; } = new();
        public Dictionary<string, int> DownNeighbors { get; set; } = new();
        public Dictionary<string, int> LeftNeighbors { get; set; } = new();
        public Dictionary<string, int> RightNeighbors { get; set; } = new();
    }
}
