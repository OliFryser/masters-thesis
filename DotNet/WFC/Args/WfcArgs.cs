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
            IReadOnlyCollection<TileWeight> weights,
            int? seed = null)
        {
            Coordinates = coordinates;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
            Weights = weights;
            Seed = seed;
        }
        
        public IReadOnlyCollection<Vector> Coordinates { get; }
        public IReadOnlyCollection<TileType> TileTypes { get; }
        public IReadOnlyCollection<AdjacencyRule> AdjacencyRules { get; }
        public IReadOnlyCollection<TileWeight> Weights { get; }
        public int? Seed { get; }
    }
}