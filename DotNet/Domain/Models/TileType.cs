using System;
using Newtonsoft.Json;

namespace Domain.Models
{
    public struct TileType : IEquatable<TileType>
    {
        public TileType(string id)
        {
            Id = id;
        }
        
        [JsonProperty]
        public string Id { get; private set; }

        public bool Equals(TileType other) => Id == other.Id;
        public override bool Equals(object? obj) => obj is TileType other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id;
    }
}