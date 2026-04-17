using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Pokémon.Args
{
    public readonly struct IndividualHandlerArgs
    {
        public static IndividualHandlerArgs Create(int mapDimensions, int tileTypeCount, List<TileType> tileTypes, List<AdjacencyRule> adjacencyRules)
        {
            List<Vector> coordinates = GetCoordinates(mapDimensions, mapDimensions).ToList();
            return new IndividualHandlerArgs(tileTypeCount, tileTypes, adjacencyRules, coordinates);
        }
        
        
        private static IEnumerable<Vector> GetCoordinates(int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return new Vector(x, y);
                }
            }
        }

        private IndividualHandlerArgs(int tileTypeCount, List<TileType> tileTypes, List<AdjacencyRule> adjacencyRules, List<Vector> coordinates)
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