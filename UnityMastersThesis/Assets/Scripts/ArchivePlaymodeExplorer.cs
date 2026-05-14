using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using MapElites.Models;
using Pokémon;
using Pokémon.Json;
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

    [SerializeField] private WfcConfig _lettersConfig;
    [SerializeField] private WfcConfig _arrowsConfig;
    [SerializeField] private WfcConfig _pokemonConfig;
    
    
    private IArchive<Key, Entry, Individual, Behavior> _archive;
    public ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> ConstrainedArchive { get; private set; }
    
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
        ConstrainedArchive = saveData.Archive;
        _coordinates = GetRectangleCoordinates(saveData.MapDimensions, saveData.MapDimensions).ToList();
        _uiHandler.Initialize(ConstrainedArchive.GetKeys());
    }
    
    private ConstrainedSaveData ReadConstrainedArchiveFile(string filename)
    {
        ConstrainedSaveData saveData = JsonSerializer.ReadConstrainedSaveDataFromFile(filename);
        return saveData;
    }

    public void BrowseConstrainedArchive(Key key, TilemapDomain tilemapDomain)
    {
        if (!ConstrainedArchive.TryGet(key, out ConstrainedEntry<Individual, Behavior> entry))
        {
            Debug.LogError($"Failed to retrieve key {key}");
            return;
        }

        WfcConfig wfcConfig = tilemapDomain switch
        {
            TilemapDomain.Letters => _lettersConfig,
            TilemapDomain.Arrows => _arrowsConfig,
            TilemapDomain.Pokemon => _pokemonConfig,
            TilemapDomain.ReadFromArchive => ConstrainedArchive.MapId switch
            {
                0 => _lettersConfig,
                1 => _arrowsConfig,
                2 => _pokemonConfig,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException(nameof(tilemapDomain), tilemapDomain, null)
        };
            
        LoadTilemap(wfcConfig);
        
        foreach (Visualizer visualizer in _visualizers)
        {
            visualizer._wfcConfig = wfcConfig;
            
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

    public enum TilemapDomain
    {
        Letters,
        Arrows,
        Pokemon,
        ReadFromArchive
    }
}