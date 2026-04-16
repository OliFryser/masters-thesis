using System.Collections.Generic;
using Domain.Models;

namespace Pokémon.Args
{
    public readonly struct IndividualHandlerArgs
    {
        public IndividualHandlerArgs(int tileTypeCount, List<TileType> tileTypes, List<AdjacencyRule> adjacencyRules, List<Vector> coordinates)
        {
            TileTypeCount = tileTypeCount;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
            Coordinates = coordinates;
        }

        public int TileTypeCount { get; }
        public List<TileType> TileTypes { get; }
        public List<AdjacencyRule> AdjacencyRules { get; }
        public List<Vector> Coordinates { get; }
    }
}