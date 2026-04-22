using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Pokémon.Args
{
    public readonly struct IndividualHandlerArgs
    {
        public static IndividualHandlerArgs Create(
            int mapDimensions, 
            int tileTypeCount, 
            List<TileType> tileTypes,
            List<AdjacencyRule> adjacencyRules,
            int evaluationIterations)
        {
            List<Vector> coordinates = 
                LevelGeneration.GetRectangleCoordinates(mapDimensions, mapDimensions).ToList();
            
            return new IndividualHandlerArgs(tileTypeCount, tileTypes, adjacencyRules, coordinates, evaluationIterations);
        }

        public IndividualHandlerArgs(
            int tileTypeCount, 
            List<TileType> tileTypes, 
            List<AdjacencyRule> adjacencyRules,
            List<Vector> coordinates,
            int evaluationIterations)
        {
            TileTypeCount = tileTypeCount;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
            Coordinates = coordinates;
            EvaluationIterations = evaluationIterations;
        }

        public int TileTypeCount { get; }
        public List<TileType> TileTypes { get; }
        public List<AdjacencyRule> AdjacencyRules { get; }
        public List<Vector> Coordinates { get; }
        public int EvaluationIterations { get; }
    }
}