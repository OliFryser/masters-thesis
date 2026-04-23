using System;
using System.ComponentModel;
using MapElites.Models;
using Pokémon.Json.Converters.Pokémon.Json;

namespace Pokémon
{
    [TypeConverter(typeof(KeyTypeConverter))]
    public class Key : BaseKey<Key>
    {
        public Key(int flowerBucket, int doorBucket, int tileTypesUsedBucket)
        {
            FlowerBucket = flowerBucket;
            DoorBucket = doorBucket;
            TileTypesUsedBucket = tileTypesUsedBucket;
        }

        public override bool Equals(Key? other) => 
            FlowerBucket == other?.FlowerBucket 
            && DoorBucket == other.DoorBucket
            && TileTypesUsedBucket == other.TileTypesUsedBucket;
        public override int GetHashCode() => HashCode.Combine(FlowerBucket, DoorBucket, TileTypesUsedBucket);

        public override string ToString()
            => $"{FlowerBucket},{DoorBucket},{TileTypesUsedBucket}";

        public static bool TryParse(string input, out Key? key)
        {
            string[] parts = input.Split(",");
            if (parts.Length != 3
                || !int.TryParse(parts[0], out int flowerBucket)
                || !int.TryParse(parts[1], out int doorBucket)
                || !int.TryParse(parts[2], out int tileTypesUsedBucket))
            {
                key = null;
                return false;
            }

            key = new Key(flowerBucket, doorBucket, tileTypesUsedBucket);
            return true;
        }

        public int FlowerBucket { get; set; }
        public int DoorBucket { get; set; }
        public int TileTypesUsedBucket { get; set; }
    }
}