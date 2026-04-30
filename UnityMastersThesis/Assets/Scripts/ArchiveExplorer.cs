using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Models;
using MapElites.Args;
using MapElites.Models;
using MapElites.Statistics;
using NaughtyAttributes;
using Pokémon;
using Pokémon.Args;
using Pokémon.Json;
using TilemapAnalysis;
using UnityEditor;
using UnityEngine;
using WFC;
using WFC.Args;
using WFC.Models;

public class ArchiveExplorer : MonoBehaviour
{
    [Header("Archive Settings")] [SerializeField]
    private int _flowerKey;

    [SerializeField] private int _doorKey;
    [SerializeField] private int _tileTypesUsedKey;

    [Header("Map Elites Configuration")] [SerializeField, Range(1, 100)]
    private int _initialIterations = 1;

    [SerializeField, Range(0, 100)] private int _mutationIterations;
    [SerializeField, Range(1, 100)] private int _evaluationIterations = 5;
    [SerializeField] private Texture2D _inputTilemap;

    [Header("Visualizer")] [SerializeField]
    private Visualizer _visualizer;

    [SerializeField] private TextAsset _archiveJsonFile;
    [SerializeField] private bool _hasArchive;

    private IArchive<Key, Entry, Individual, Behavior> _archive;

    private void OnValidate()
    {
        HasArchive();
    }

    [Button]
    public void GenerateArchive()
    {
        string tilemapPath = AssetDatabase.GetAssetPath(_inputTilemap);
        const int mapDimensions = 20;
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        int tileTypeCount = tilemapAnalyzer.TileTypeCount;
        List<Domain.Models.AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();
        KeyCeilings keyCeilings = new KeyCeilings(
            flowerPercentageCeiling: 0.2f,
            doorPercentageCeiling: 0.05f,
            variationPercentageCeiling: 1.0f);

        IndividualHandlerArgs individualHandlerArgs =
            IndividualHandlerArgs.Create(mapDimensions, tileTypeCount, tileTypes, adjacencyRules, _evaluationIterations,
                keyCeilings);

        IndividualHandler individualHandler = new(individualHandlerArgs);

        MapElitesArgs args = new MapElitesArgs(_initialIterations, _mutationIterations, Debug.Log,
            $"Assets/Output/{DateTime.Now:yyyyMMdd_HHmmss}", new List<IStatisticsTracker>());

        _archive = MapElites.MapElites.Run(individualHandler, args);

        PrintKeys();

        // SaveToJson(_archive);
        int maxFlowerBucket = _archive.GetKeys().Max(k => k.FlowerBucket);
        int maxTileTypesUsedKey = _archive.GetKeys().Max(k => k.TileTypesUsedBucket);
        
        _flowerKey = maxFlowerBucket;
        _tileTypesUsedKey = maxTileTypesUsedKey;
    }

    [Button]
    public void LoadArchiveFromFile()
    {
        string path = AssetDatabase.GetAssetPath(_archiveJsonFile);
        SaveData saveData = JsonSerializer.ReadFromFile(path);
        _archive = saveData.Archive;
        Debug.Log($"Archive has been loaded.");
    }

    [Button]
    public void VisualizeFromBucketKeys()
    {
        if (!HasArchive())
        {
            Debug.LogWarning("Archive has not been generated.");
            return;
        }

        Key key = new Key(_flowerKey, _tileTypesUsedKey);
        if (_archive.TryGet(key, out Entry entry))
        {
            WfcArgs args = entry.Individual.GetWfcArgs(_inputTilemap);
            State state = WaveFunctionCollapse.Run(args);
            _visualizer.Display(state);
        }
    }

    [Button]
    public void PrintKeys()
    {
        if (!HasArchive())
        {
            Debug.LogWarning("Archive has not been generated.");
            return;
        }

        int c = 0;
        foreach (Key archiveKey in _archive.GetKeys())
        {
            print(
                $"Key {c}: Flower {archiveKey.FlowerBucket}, Tiles {archiveKey.TileTypesUsedBucket}");
            c++;
        }
    }

    private bool HasArchive()
    {
        _hasArchive = _archive != null;
        return _hasArchive;
    }
}