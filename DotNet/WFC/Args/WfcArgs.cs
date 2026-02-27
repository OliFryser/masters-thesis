using System.Collections.Generic;
using Domain.Models;

namespace WFC.Args
{
    public class WfcArgs
    {
        public WfcArgs(
            IReadOnlyCollection<Vector> coordinates, 
            IReadOnlyCollection<TileType> tileTypes,
            IReadOnlyCollection<AdjacencyRule> adjacencyRules)
        {
            Coordinates = coordinates;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
        }

        public IReadOnlyCollection<Vector> Coordinates { get; }
        public IReadOnlyCollection<TileType> TileTypes { get; }
        public IReadOnlyCollection<AdjacencyRule> AdjacencyRules { get; }
    }
}