using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain.Models;
using MapElites.Args;
using MapElites.Models;
using NaughtyAttributes;
using Pokémon;
using Pokémon.Args;
using TilemapAnalysis;
using UnityEditor;
using UnityEngine;
using WFC;
using WFC.Args;
using WFC.Models;

public class ArchiveExplorer : MonoBehaviour
{
    [Header("Archive Settings")] 
    [SerializeField] private int _flowerKey;
    [SerializeField] private int _doorKey;
    [SerializeField] private int _tileTypesUsedKey;
    
    [Header("Map Elites Configuration")]
    [SerializeField, Range(1, 100)] private int _initialIterations = 1;
    [SerializeField, Range(0, 100)] private int _mutationIterations;
    [SerializeField, Range(1, 100)] private int _evaluationIterations = 5;
    [SerializeField] private Texture2D _inputTilemap;
    
    [Header("Visualizer")]
    [SerializeField] private Visualizer _visualizer;
    [SerializeField] private bool _hasArchive;
    
    private Archive<Key, Entry, Individual, Behavior> _archive;

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

        IndividualHandlerArgs individualHandlerArgs =
            IndividualHandlerArgs.Create(mapDimensions, tileTypeCount, tileTypes, adjacencyRules, _evaluationIterations);
        
        IndividualHandler individualHandler = new(individualHandlerArgs);
        
        MapElitesArgs args = new MapElitesArgs(_initialIterations, _mutationIterations, Debug.Log, $"Assets/Output/{DateTime.Now:yyyyMMdd_HHmmss}");
        
        _archive = MapElites.MapElites.Run(individualHandler, args);

        int c = 0;
        foreach (Key archiveKey in _archive.Keys)
        {
            print($"Key {c}: Flower {archiveKey.FlowerBucket}, Door {archiveKey.DoorBucket}, Tiles {archiveKey.TileTypesUsedBucket}");
            c++;
        }
        
        
        
        // SaveToJson(_archive);

        int maxDoorBucket = _archive.Keys.Max(k => k.DoorBucket);
        int maxFlowerBucket = _archive.Keys.Max(k => k.FlowerBucket);
        int maxTileTypesUsedKey = _archive.Keys.Max(k => k.TileTypesUsedBucket);
        
        _doorKey = maxDoorBucket;
        _flowerKey = maxFlowerBucket;
        _tileTypesUsedKey = maxTileTypesUsedKey;
    }

    [Button]
    public void VisualizeMaxFitness()
    {
        if (!HasArchive())
        {
            Debug.LogWarning("Archive has not been generated.");
            return;
        }

        Individual maxFitnessIndividual = _archive.GetMaxFitnessIndividual();
        State state = GetState(maxFitnessIndividual);
        _visualizer.Display(state);
    }

    [Button]
    public void VisualizeFromBucketKeys()
    {
        if (!HasArchive())
        {
            Debug.LogWarning("Archive has not been generated.");
            return;
        }
        
        Key key = new Key(_flowerKey, _doorKey, _tileTypesUsedKey);
        if (_archive.TryGet(key, out Entry entry))
        {
            State state = GetState(entry.Individual);
            _visualizer.Display(state);
        }
    }

    private bool HasArchive()
    {
        _hasArchive = _archive != null;
        return _hasArchive;
    }

    private State GetState(Individual individual)
    {
        WfcArgs args = GetWfcArgs(individual);
        State state = WaveFunctionCollapse.Run(args);
        return state;
    }

    private WfcArgs GetWfcArgs(Individual individual)
    {
        const int mapDimensions = 20;
        List<Vector> coordinates = Pokémon.LevelGeneration.GetRectangleCoordinates(mapDimensions, mapDimensions).ToList();
        
        string tilemapPath = AssetDatabase.GetAssetPath(_inputTilemap);
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        List<Domain.Models.AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules()
            .Concat(tilemapAnalyzer.GetSymmetryRules()).ToList();
        
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, individual.Weights, individual.Seed);

        return args;
    }
}