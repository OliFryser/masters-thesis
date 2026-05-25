using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using MapElites.Models;
using Domain;
using Domain.Json;
using UnityEngine;
using WFC.Args;
using static Domain.LevelGeneration;

public class ArchivePlaymodeExplorer : MonoBehaviour
{
    [SerializeField] private Visualizer[] _visualizers;
    [SerializeField] private Texture2D _tilemap;
    [SerializeField] private UIHandler _uiHandler;

    [SerializeField] private WfcConfig _lettersConfig;
    [SerializeField] private WfcConfig _arrowsConfig;
    [SerializeField] private WfcConfig _pokemonConfig;

    private IArchive<Key, Entry, Individual, Behavior> _archive;

    public ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> ConstrainedArchive
    {
        get;
        private set;
    }

    private IReadOnlyCollection<TileType> _tileTypes;
    private IReadOnlyCollection<Core.Models.AdjacencyRule> _adjacencyRules;
    private List<Vector> _coordinates;

    public void LoadTilemap(WfcConfig wfcConfig)
    {
        var args = wfcConfig.ToArgs();

        _tileTypes = args.TileTypes;
        _adjacencyRules = args.AdjacencyRules;
    }

    public void LoadArchiveFile(string filename)
    {
        ConstrainedSaveData saveData = JsonSerializer.ReadConstrainedSaveDataFromFile(filename);
        SetSaveData(saveData);
    }

    public void LoadArchive(string json)
    {
        ConstrainedSaveData saveData = JsonSerializer.ReadConstrainedSaveDataFromJson(json);
        SetSaveData(saveData);
    }

    private void SetSaveData(ConstrainedSaveData saveData)
    {
        ConstrainedArchive = saveData.Archive;
        _coordinates = GetRectangleCoordinates(saveData.MapDimensions, saveData.MapDimensions).ToList();
        _uiHandler.Initialize(ConstrainedArchive);
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

        Vector3 size = tilemapDomain switch
        {
            TilemapDomain.Letters => Vector3.one * 4,
            TilemapDomain.Arrows => Vector3.one * 4,
            TilemapDomain.Pokemon => Vector3.one,
            TilemapDomain.ReadFromArchive => Vector3.one,
            _ => throw new ArgumentOutOfRangeException(nameof(tilemapDomain), tilemapDomain, null)
        };

        foreach (Visualizer visualizer in _visualizers)
        {
            visualizer.transform.localScale = size;

            visualizer.WfcConfig = wfcConfig;

            WfcArgs args = new WfcArgs(_coordinates, _tileTypes, _adjacencyRules, entry.Individual.Weights);

            _ = visualizer.Animate(args);
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