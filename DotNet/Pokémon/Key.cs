using System;
using System.ComponentModel;
using MapElites.Models;
using Pokémon.Json.Converters.Pokémon.Json;

namespace Pokémon
{
    [TypeConverter(typeof(KeyTypeConverter))]
    public class Key : BaseKey<Key>
    {
        public Key(int flowerBucket, int tileTypesUsedBucket)
        {
            FlowerBucket = flowerBucket;
            TileTypesUsedBucket = tileTypesUsedBucket;
        }

        public override bool Equals(Key? other) => 
            FlowerBucket == other?.FlowerBucket
            && TileTypesUsedBucket == other.TileTypesUsedBucket;
        public override int GetHashCode() => HashCode.Combine(FlowerBucket, TileTypesUsedBucket);

        public override string ToString()
            => $"{FlowerBucket},{TileTypesUsedBucket}";

        public static bool TryParse(string input, out Key? key)
        {
            string[] parts = input.Split(",");
            if (parts.Length != 2
                || !int.TryParse(parts[0], out int flowerBucket)
                || !int.TryParse(parts[1], out int tileTypesUsedBucket))
            {
                key = null;
                return false;
            }

            key = new Key(flowerBucket, tileTypesUsedBucket);
            return true;
        }

        public int FlowerBucket { get; set; }
        public int TileTypesUsedBucket { get; set; }
    }
}