using System;

namespace WFC.Models
{
    public struct TileType : IEquatable<TileType>
    {
        public TileType(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public bool Equals(TileType other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is TileType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}