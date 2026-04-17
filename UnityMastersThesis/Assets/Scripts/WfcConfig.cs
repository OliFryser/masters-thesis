using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using UnityEngine;
using UnityEngine.Tilemaps;
using WFC.Args;
using WFC.Extensions;

[CreateAssetMenu(fileName = "WfcArgs", menuName = "Scriptable Objects/Wave Function Collapse Args")]
public class WfcConfig : ScriptableObject
{
    public AdjacencyRule[] Rules;
    public TileBase[] Tiles;
    public SerializedDictionary<string, int> TileTypeToCount;
    public int TileCount;
    public int Width = 10;
    public int Height = 10;

    public WfcArgs ToArgs()
    {
        List<Vector> positions = new();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                positions.Add(new Vector(x, y));
            }
        }

        List<Domain.Models.AdjacencyRule> rules = new();
        foreach (var rule in Rules)
        {
            TileType fromTile = new TileType(rule.From.name);
            TileType toTile = new TileType(rule.To.name);

            rules.Add(new Domain.Models.AdjacencyRule(fromTile, toTile, rule.Direction));
        }

        var tileTypeToCount =
            TileTypeToCount.ToDictionary().ToDictionary(kvp => new TileType(kvp.Key), kvp => kvp.Value);

        List<TileType> tiles = tileTypeToCount.Select(kvp => new TileType(kvp.Key.Id)).ToList();
        
        return new WfcArgs(positions, tiles, rules, tileTypeToCount);
    }
}

[Serializable]
public struct AdjacencyRule
{
    public AdjacencyRule(TileBase from, TileBase to, Direction direction)
    {
        From = from;
        To = to;
        Direction = direction;
    }

    public TileBase From;
    public TileBase To;
    public Direction Direction;
}