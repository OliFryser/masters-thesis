using System;
using MapElites.Models;

namespace Pokémon
{
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
            if (input.Length != 3
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

        private int FlowerBucket { get; set; }
        private int DoorBucket { get; set; }
        private int TileTypesUsedBucket { get; set; }
    }
}