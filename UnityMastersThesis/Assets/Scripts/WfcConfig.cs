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
    public List<SerializedTileWeight> Weights;
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

        List<TileWeight> tileWeights = Weights.Select(w => w.ToTileWeight()).ToList();

        List<TileType> tiles = tileWeights.Select(tileWeight => tileWeight.TileType).ToList();

        return new WfcArgs(positions, tiles, rules, tileWeights);
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

[Serializable]
public struct SerializedTileWeight
{
    public SerializedTileWeight(TileWeight tileWeight)
    {
        Weight = tileWeight.Weight;
        TileTypeId = tileWeight.TileType.Id;
    }

    public TileWeight ToTileWeight()
        => new TileWeight(new TileType(TileTypeId), Weight);

    public double Weight;
    public string TileTypeId;
}