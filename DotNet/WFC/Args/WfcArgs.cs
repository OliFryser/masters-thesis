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
            int maxPropagationDepth = 5)
        {
            Coordinates = coordinates;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
            MaxPropagationDepth = maxPropagationDepth;
        }
        
        public int MaxPropagationDepth { get; }
        public IReadOnlyCollection<Vector> Coordinates { get; }
        public IReadOnlyCollection<TileType> TileTypes { get; }
        public IReadOnlyCollection<AdjacencyRule> AdjacencyRules { get; }
    }
}