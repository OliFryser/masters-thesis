using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Level
    {
        public Level(ICollection<Cell> cells)
        {
            Dictionary<Vector, Cell> dictionary = cells.ToDictionary(cell => cell.Position);
            CellLookup = dictionary;
        }

        public IDictionary<Vector, Cell> CellLookup { get; }
        public ICollection<Cell> Cells => CellLookup.Values;
    }

    public class Cell
    {
        public Cell(Vector position, HashSet<TileOption> options)
        {
            Position = position;
            Options = options;
        }

        public Vector Position { get; }
        public HashSet<TileOption> Options { get; }
        public float Entropy { get; set; }

        public void CalculateEntropy()
        {
            Entropy = -Options.Aggregate<TileOption, float>(0,
                (acc, tileOption) => acc + tileOption.Weight * MathF.Log(tileOption.Weight));
        }
    }

    public readonly struct Vector : IEquatable<Vector>
    {
        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }
        public float Y { get; }

        public bool Equals(Vector other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object? obj) => obj is Vector other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
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