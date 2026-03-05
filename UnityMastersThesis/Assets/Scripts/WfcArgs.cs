using System;
using System.Collections.Generic;
using Domain.Models;
using Models;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using WFC.Args;
using WFC.Extensions;
using WFC.Output;
using Tile = Domain.Models.Tile;

[CreateAssetMenu(fileName = "WfcArgs", menuName = "Scriptable Objects/Wave Function Collapse Args")]
public class WfcArgs : ScriptableObject
{
    // TODO: Put this somewhere else
    private const string BaseDirectory = "Assets/Resources/Tiles/Pokemon/";
    
    public AdjacencyRule[] Rules;
    public int Width = 100;
    public int Height = 200;

    [Button("Run Wave Function Collapse")]
    public List<Tile> RunWfc()
    {
        Result result = WFC.WaveFunctionCollapse.Run(ToArgs());
        return result.Map?.Tiles ?? new List<Tile>();
    }

    private WFC.Args.WfcArgs ToArgs()
    {
        List<Vector> positions = new();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                positions.Add(new Vector(x, y));
            }
        }
        HashSet<TileType> tileIds = new HashSet<TileType>();
        List<WFC.Args.AdjacencyRule> rules = new();
        foreach (var rule in Rules)
        {
            TileType fromTile = new TileType(rule.From.name);
            TileType toTile = new TileType(rule.To.name);

            tileIds.Add(fromTile);
            tileIds.Add(toTile);
            
            rules.Add(new WFC.Args.AdjacencyRule(fromTile, toTile, rule.Direction));
            rules.Add(new WFC.Args.AdjacencyRule(toTile, fromTile, rule.Direction.Reverse()));
        }
        List<TileType> tiles = new(tileIds);
        return new WFC.Args.WfcArgs(positions, tiles, rules);
    }
}

[Serializable]
public struct AdjacencyRule
{
    public TileBase From;
    public TileBase To;
    public Direction Direction;
}