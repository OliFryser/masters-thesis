using System;

namespace WFC.Args
{
    public struct TileType : IEquatable<TileType>
    {
        public TileType(uint id)
        {
            Id = id;
        }

        public uint Id { get; }

        public bool Equals(TileType other) => Id == other.Id;
        public override bool Equals(object? obj) => obj is TileType other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();
    }
}