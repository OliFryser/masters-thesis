using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using System.Threading;
using DefaultNamespace;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using WFC;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;
using Tile = Core.Models.Tile;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Visualizer : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    public WfcConfig WfcConfig;

    [SerializeField] private FloatScriptableObject _animationSpeed;
    [SerializeField] private TileBase _emptyTile;

    private State _state;
    private List<EmptyTile> _emptyTiles;
    private CancellationTokenSource _cancellationTokenSource;
    [SerializeField] private float _maxWaitTime = 1f;
    
    public async Awaitable Animate(WfcArgs wfcArgs)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        State state = wfcArgs.ToState();
        
        while (!token.IsCancellationRequested && !state.IsCollapsed)
        {
            if (state.HasReachedContradiction)
            {
                await Awaitable.WaitForSecondsAsync(1f, token);
                state = wfcArgs.ToState();
            }
            
            float wait = Mathf.Lerp(_maxWaitTime, 0.0001f, _animationSpeed.Value);

            if (wait > 0.1f)
            {
                state = WaveFunctionCollapse.Step(state);
                DisplayTiles(state);
                await Awaitable.WaitForSecondsAsync(wait, token);
            }
            else
            {
                int steps = 10 - (int)(wait * 100f);
                for (int i = 0; i < steps; i++)
                { 
                    state = WaveFunctionCollapse.Step(state);
                }
                
                DisplayTiles(state);
                await Awaitable.NextFrameAsync(token);
            }
        }
    }

    private void DisplayTiles(State state)
    {
        _tilemap.ClearAllTiles();

        List<Tile> tileLayout = state.GetMap().Tiles;

        foreach (Tile tile in tileLayout)
        {
            try
            {
                TileBase tileBase = WfcConfig.Tiles.First(tileBase => tileBase.name == tile.Type.Id);
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

            float truncatedEntropy = (float)tile.Entropy;
            
            Color color = tile.Options == 0
                ? Color.magenta
                : new Color(truncatedEntropy, truncatedEntropy, truncatedEntropy, 1f);

            _tilemap.SetColor(position, color);
        }
    }

    private void InitializeState()
    {
        if (_state == null)
        {
            _state = WfcConfig.ToArgs().ToState();
            Debug.Log("Creating initial state.");
        }
    }
    
    public void Display(State state)
    {
        _state = state;
        DisplayTiles(state);
    }

#if UNITY_EDITOR
    private float _lastStepTime;
    
    [Button("Step")]
    public void Step()
    {
        InitializeState();

        _state = WaveFunctionCollapse.Step(_state);
        DisplayTiles(_state);
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

    [Button("Reset And Complete")]
    public void ResetAndComplete()
    {
        Reset();
        Complete();
    }


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
        if (currentTime - _lastStepTime >= _animationSpeed.Value)
        {
            for (int i = 0; i < 20; i++)
            {
                Step();
            }
            
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