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


        public int FlowerBucket { get; }
        public int DoorBucket { get; }
        public int TileTypesUsedBucket { get; } // Add in .ToString()
    }
}