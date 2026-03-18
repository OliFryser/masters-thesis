using System.Collections.Generic;
using Domain.Models;

namespace WFC.Args
{
    public class WfcArgs
    {
        public WfcArgs(
            IReadOnlyCollection<Vector> coordinates, 
            IReadOnlyCollection<TileType> tileTypes,
            IReadOnlyCollection<AdjacencyRule> adjacencyRules,
            IReadOnlyDictionary<TileType, int> tileTypeToCount,
            int tileCount,
            int maxPropagationDepth = 5)
        {
            Coordinates = coordinates;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
            TileTypeToCount = tileTypeToCount;
            TileCount = tileCount;
            MaxPropagationDepth = maxPropagationDepth;
        }
        
        public int MaxPropagationDepth { get; }
        public IReadOnlyCollection<Vector> Coordinates { get; }
        public IReadOnlyCollection<TileType> TileTypes { get; }
        public IReadOnlyCollection<AdjacencyRule> AdjacencyRules { get; }
        public IReadOnlyDictionary<TileType, int> TileTypeToCount { get; }
        public int TileCount { get; }
    }
}