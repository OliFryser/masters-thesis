using System;
using System.Collections.Generic;
using Domain.Models;
using Models;
using UnityEngine;
using UnityEngine.Tilemaps;
using WFC.Args;
using WFC.Extensions;

[CreateAssetMenu(fileName = "WfcArgs", menuName = "Scriptable Objects/Wave Function Collapse Args")]
public class WfcArgs : ScriptableObject
{
    public AdjacencyRule[] Rules;
    public int Width = 100;
    public int Height = 200;

    public WFC.Args.WfcArgs ToArgs()
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