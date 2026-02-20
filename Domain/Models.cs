using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Level
    {
        public Level(IReadOnlyCollection<Cell> cells)
        {
            Cells = cells;
        }

        public IReadOnlyCollection<Cell> Cells { get; }
    }

    public class Cell
    {
        public Cell(Vector position, HashSet<TileOption> options)
        {
            Position = position;
            Options = options;
            Entropy = -options.Aggregate<TileOption, float>(0,
                (acc, tileOption) => acc + tileOption.Weight * MathF.Log(tileOption.Weight));
        }

        public Vector Position { get; }
        public HashSet<TileOption> Options { get; }
        public float Entropy { get; set; }
    }

    public struct Vector
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class TileType
    {
        public string Type { get; set; } = "Default";
    }

    public class TileOption
    {
        public TileType TileType { get; set; }
        public float Weight { get; set; } = 1f;
    }

    public class TileRules
    {
        public Dictionary<CardinalDirection, HashSet<AdjacencyRule>> AdjacencyRules =
            new Dictionary<CardinalDirection, HashSet<AdjacencyRule>>();
    }

    public class AdjacencyRule
    {
        public TileType Type { get; set; } = new TileType();
        public float Weight { get; set; } = 1;
    }

    public enum CardinalDirection
    {
        North,
        East,
        South,
        West
    }
}