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


        public int FlowerBucket { get; }
        public int DoorBucket { get; }
    }
}