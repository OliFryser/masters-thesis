using System;

namespace WFC.Models
{
    public struct TileType : IEquatable<TileType>
    {
        public TileType(uint id)
        {
            Id = id;
        }

        public uint Id { get; set; }

        public bool Equals(TileType other) => Id == other.Id;
        public override bool Equals(object? obj) => obj is TileType other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();
    }
}