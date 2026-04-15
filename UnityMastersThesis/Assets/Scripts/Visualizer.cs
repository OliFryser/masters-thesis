using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
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
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private WfcConfig _wfcConfig;

    [SerializeField, Range(0f, 1f)] private float _animationSpeed = .5f;
    [SerializeField] private TileBase _emptyTile;

    private State _state;
    private List<EmptyTile> _emptyTiles;
    
    private void DisplayTiles(State state)
    {
        _tilemap.ClearAllTiles();

        List<Tile> tileLayout = state.GetMap().Tiles;

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

        DisplayEmptyTiles(state.EmptyTiles);
        _emptyTiles = state.EmptyTiles;
    }

    private void DisplayEmptyTiles(List<EmptyTile> tiles)
    {
        foreach (EmptyTile tile in tiles)
        {
            Vector3Int position = new Vector3Int(tile.Position.X, -tile.Position.Y);
            _tilemap.SetTile(position, _emptyTile);

            Color color = tile.Options == 0
                ? Color.magenta
                : new Color(tile.Entropy, tile.Entropy, tile.Entropy, 1f);

            _tilemap.SetColor(position, color);
        }
    }

    [Button("Step")]
    public void Step()
    {
        InitializeState();

        _state = WaveFunctionCollapse.Step(_state);
        DisplayTiles(_state);
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
        DisplayTiles(_state);
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
    
    private void OnDrawGizmos()
    {
        if (_emptyTiles == null || _tilemap == null) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(0.4f, 0.2f, 0.6f);
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;

        foreach (EmptyTile emptyTile in _emptyTiles)
        {
            Vector3Int position = new Vector3Int(emptyTile.Position.X, -emptyTile.Position.Y, 0);
            Vector3 worldPos = _tilemap.CellToWorld(position) + _tilemap.tileAnchor;
            Handles.Label(worldPos, $"{emptyTile.Options}", style);
        }
    }
#endif
}