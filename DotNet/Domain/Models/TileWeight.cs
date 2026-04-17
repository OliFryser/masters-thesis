using System;

namespace Domain.Models
{
    public struct TileWeight
    {
        public TileWeight(TileType tileType, int weight)
        {
            TileType = tileType;
            Weight = weight;
        }
        
        public TileType TileType { get; }
        public int Weight { get; }
    }
}