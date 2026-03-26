using System;

namespace Pokémon
{
    public class Key : IEquatable<Key>
    {
        public Key(int flowerBucket, int doorBucket)
        {
            FlowerBucket = flowerBucket;
            DoorBucket = doorBucket;
        }

        public bool Equals(Key other) => FlowerBucket == other.FlowerBucket && DoorBucket == other.DoorBucket;
        
        
        public int FlowerBucket { get; }
        public int DoorBucket { get; }
    }
}