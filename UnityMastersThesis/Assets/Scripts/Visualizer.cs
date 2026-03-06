using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
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
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private WfcConfig _wfcConfig;

    [SerializeField, Range(0f, 1f)] private float _animationSpeed = .5f;
    
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
                TileBase tileBase = _wfcConfig.Tiles.First(tileBase => tileBase.name == tile.Type.Id);
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
        
        WfcArgs wfcArgs = _wfcConfig.ToArgs();
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
            _state = _wfcConfig.ToArgs().ToState();
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
        StopAnimationInEditor();
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
    public void StopAnimationInEditor()
    {
        EditorApplication.update -= UpdateEditorAnimation;
    }

    private void UpdateEditorAnimation()
    {
        if (_state != null && _state.IsCollapsed)
        {
            StopAnimationInEditor();
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
}
