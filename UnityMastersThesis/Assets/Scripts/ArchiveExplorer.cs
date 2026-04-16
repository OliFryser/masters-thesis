using System;
using System.Linq;
using MapElites.Args;
using MapElites.Models;
using NaughtyAttributes;
using Pokémon;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ArchiveExplorer : MonoBehaviour
{
    [Header("Archive Settings")] 
    [SerializeField] private int _flowerKey;
    [SerializeField] private int _doorKey;
    
    [Header("Map Elites Configuration")]
    [SerializeField, Range(1, 100)] private int _initialIterations = 1;
    [SerializeField, Range(0, 100)] private int _mutationIterations;
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
        IndividualHandler individualHandler = new(tilemapPath);
        
        MapElitesArgs args = new MapElitesArgs(_initialIterations, _mutationIterations, Debug.Log);
        
        _archive = MapElites.MapElites.Run(individualHandler, args);

        int maxDoorBucket = _archive.Keys.Max(k => k.DoorBucket);
        int maxFlowerBucket = _archive.Keys.Max(k => k.FlowerBucket);
        
        _doorKey = maxDoorBucket;
        _flowerKey = maxFlowerBucket;
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
        
        _visualizer.Display(maxFitnessIndividual);
    }

    [Button]
    public void VisualizeFromBucketKeys()
    {
        if (!HasArchive())
        {
            Debug.LogWarning("Archive has not been generated.");
            return;
        }
        
        Key key = new Key(_flowerKey, _doorKey);
        if (_archive.TryGet(key, out Entry entry))
        {
            _visualizer.Display(entry.Individual);
        }
    }

    private bool HasArchive()
    {
        _hasArchive = _archive != null;
        return _hasArchive;
    }
}