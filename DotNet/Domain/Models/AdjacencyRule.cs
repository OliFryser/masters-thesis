using System;

namespace Domain.Models
{
    public struct AdjacencyRule : IEquatable<AdjacencyRule>
    {
        public AdjacencyRule(TileType from, TileType to, Direction direction)
        {
            From = from;
            To = to;
            Direction = direction;
        }

        public TileType From { get; }
        public TileType To { get; }
        public Direction Direction { get; }

        public bool Equals(AdjacencyRule other)
        {
            return From.Equals(other.From) && To.Equals(other.To) && Direction == other.Direction;
        }

        public override bool Equals(object? obj)
        {
            return obj is AdjacencyRule other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To, (int)Direction);
        }
    }
}