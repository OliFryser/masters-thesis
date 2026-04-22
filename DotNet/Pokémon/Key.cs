using System;
using MapElites.Models;

namespace Pokémon
{
    public class Key : BaseKey<Key>
    {
        public Key(int flowerBucket, int doorBucket)
        {
            FlowerBucket = flowerBucket;
            DoorBucket = doorBucket;
        }

        public override bool Equals(Key? other) => FlowerBucket == other?.FlowerBucket && DoorBucket == other.DoorBucket;
        public override int GetHashCode() => HashCode.Combine(FlowerBucket, DoorBucket);

        public override string ToString()
            => $"{FlowerBucket},{DoorBucket}";

        public static bool TryParse(string input, out Key? key)
        {
            string[] parts = input.Split(",");
            if (input.Length != 2
                || !int.TryParse(parts[0], out int flowerBucket)
                || !int.TryParse(parts[1], out int doorBucket))
            {
                key = null;
                return false;
            }

            key = new Key(flowerBucket, doorBucket);
            return true;
        }

        private int FlowerBucket { get; set; }
        private int DoorBucket { get; set; }
    }
}