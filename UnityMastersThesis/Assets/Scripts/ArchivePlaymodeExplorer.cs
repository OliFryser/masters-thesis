using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using MapElites.Models;
using Pokémon;
using Pokémon.Json;
using TilemapAnalysis;
using UnityEngine;
using WFC;
using WFC.Args;
using WFC.Models;
using static Pokémon.LevelGeneration;

public class ArchivePlaymodeExplorer : MonoBehaviour
{
    [SerializeField] private Visualizer[] _visualizers;
    [SerializeField] private Texture2D _tilemap;
    [SerializeField] private UIHandler _uiHandler;
    [SerializeField] private WfcConfig[] _wfcConfigs; 
    
    private IArchive<Key, Entry, Individual, Behavior> _archive;
    private ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> _constrainedArchive;
    
    private IReadOnlyCollection<TileType> _tileTypes;
    private IReadOnlyCollection<Domain.Models.AdjacencyRule> _adjacencyRules;
    private List<Vector> _coordinates;

    public void LoadTilemap(WfcConfig wfcConfig)
    {
        var args = wfcConfig.ToArgs();
        
        _tileTypes = args.TileTypes;
        _adjacencyRules = args.AdjacencyRules;
    }
    
    public void LoadArchive(string filename)
    {
        ConstrainedSaveData saveData = ReadConstrainedArchiveFile(filename);
        _constrainedArchive = saveData.Archive;
        _coordinates = GetRectangleCoordinates(saveData.MapDimensions, saveData.MapDimensions).ToList();
        _uiHandler.Initialize(_constrainedArchive.GetKeys());
    }
    
    private ConstrainedSaveData ReadConstrainedArchiveFile(string filename)
    {
        ConstrainedSaveData saveData = JsonSerializer.ReadConstrainedSaveDataFromFile(filename);
        return saveData;
    }

    public void BrowseConstrainedArchive(Key key, int wfcConfigIndex)
    {
        if (!_constrainedArchive.TryGet(key, out ConstrainedEntry<Individual, Behavior> entry))
        {
            Debug.LogError($"Failed to retrieve key {key}");
            return;
        }

        if (wfcConfigIndex >= _wfcConfigs.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(wfcConfigIndex), "WFC Config Index is out of range. Make sure 3 wfc configs are added");
        }
        
        var currentWfcConfig = _wfcConfigs[wfcConfigIndex];
        LoadTilemap(currentWfcConfig);
        
        foreach (Visualizer visualizer in _visualizers)
        {
            visualizer._wfcConfig = _wfcConfigs[wfcConfigIndex];
            
            WfcArgs args = new WfcArgs(_coordinates, _tileTypes, _adjacencyRules, entry.Individual.Weights);

            State state = WaveFunctionCollapse.Run(args);

            const int limit = 100;
            int c = 0;
            while (!state.IsCollapsed)
            {
                state = WaveFunctionCollapse.Run(args);
                if (c++ >= limit)
                {
                    break;
                }
            }

            visualizer.Display(state);
        }
    }
}