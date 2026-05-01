using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using MapElites.Models;
using NaughtyAttributes;
using Pokémon;
using Pokémon.Json;
using TilemapAnalysis;
using UnityEditor;
using UnityEngine;
using WFC;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;
using static Pokémon.LevelGeneration;

public class ArchivePlaymodeExplorer : MonoBehaviour
{
    [SerializeField] private Visualizer[] _visualizers;
    [SerializeField] private TextAsset _archiveFile;
    [SerializeField] private TextAsset _constrainedArchiveFile;
    [SerializeField] private Texture2D _tilemap;
    [SerializeField] private UIHandler _uiHandler;
    
    private IArchive<Key, Entry, Individual, Behavior> _archive;
    private IArchive<Key, Entry, Individual, Behavior> _constrainedArchive;
    
    private List<TileType> _tileTypes;
    private List<Domain.Models.AdjacencyRule> _adjacencyRules;
    private List<Vector> _coordinates;

    private void Start()
    {
        (List<TileType> tileTypes, List<Domain.Models.AdjacencyRule> adjacencyRules) = AnalyzeTilemap();

        _tileTypes = tileTypes;
        
        _adjacencyRules = adjacencyRules;

        (IArchive<Key, Entry, Individual, Behavior> archive, int mapDimension) = ReadArchiveFile(_archiveFile);

        _archive = archive;
        
        (IArchive<Key, Entry, Individual, Behavior> constrainedArchive, int _) = ReadArchiveFile(_constrainedArchiveFile);

        _constrainedArchive = constrainedArchive;

        _coordinates = GetRectangleCoordinates(mapDimension, mapDimension).ToList();

        _uiHandler.Initialize(_archive.GetKeys());
    }

    private (List<TileType> tileTypes, List<Domain.Models.AdjacencyRule> adjacencyRules) AnalyzeTilemap()
    {
        string tilemapPath = AssetDatabase.GetAssetPath(_tilemap);
        
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
        
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        
        List<Domain.Models.AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules()
            .Concat(tilemapAnalyzer.GetSymmetryRules()).ToList();

        return (tileTypes, adjacencyRules);
    }
    
    private (IArchive<Key, Entry, Individual, Behavior>, int MapDimension) ReadArchiveFile(TextAsset archiveFile)
    {
        string path = AssetDatabase.GetAssetPath(_archiveFile);
        
        SaveData saveData = JsonSerializer.ReadFromFile(path);

        return (saveData.Archive, saveData.MapDimension);
    }

    public void BrowseArchive(Key key) => Browse(_archive, key);
    
    public void BrowseConstrainedArchive(Key key) => Browse(_constrainedArchive, key);

    private void Browse(IArchive<Key, Entry, Individual, Behavior> archive, Key key)
    {
        if (!archive.TryGet(key, out Entry entry))
        {
            Debug.LogError($"Failed to retrieve key {key}");
            return;
        }
        
        foreach (Visualizer visualizer in _visualizers)
        {
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