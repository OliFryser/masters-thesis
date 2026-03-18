using System;
using System.Collections.Generic;
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
    
    public Dictionary<TileType, int> TileTypeToCount;
    public int TileCount = 0;
    public int Width = 50;
    public int Height = 50;
    public int MaxPropagationDepth = 5;
    
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
        HashSet<TileType> tileIds = new HashSet<TileType>();
        List<Domain.Models.AdjacencyRule> rules = new();
        foreach (var rule in Rules)
        {
            TileType fromTile = new TileType(rule.From.name);
            TileType toTile = new TileType(rule.To.name);

            tileIds.Add(fromTile);
            tileIds.Add(toTile);
            
            rules.Add(new Domain.Models.AdjacencyRule(fromTile, toTile, rule.Direction));
            rules.Add(new Domain.Models.AdjacencyRule(toTile, fromTile, rule.Direction.Reverse()));
        }
        List<TileType> tiles = new(tileIds);
        return new WfcArgs(positions, tiles, rules, TileTypeToCount, TileCount, MaxPropagationDepth);
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