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
            int evaluationIterations,
            KeyCeilings keyCeilings,
            int numberOfBucketsPerAxis,
            double standardDeviation)
        {
            List<Vector> coordinates =
                LevelGeneration.GetRectangleCoordinates(mapDimensions, mapDimensions).ToList();

            return new IndividualHandlerArgs(tileTypeCount, tileTypes, adjacencyRules, coordinates,
                mapDimensions, evaluationIterations, keyCeilings, numberOfBucketsPerAxis, standardDeviation);
        }

        private IndividualHandlerArgs(
            int tileTypeCount,
            List<TileType> tileTypes,
            List<AdjacencyRule> adjacencyRules,
            List<Vector> coordinates,
            int mapDimensions,
            int evaluationIterations,
            KeyCeilings keyCeilings, int numberOfBucketsPerAxis, double standardDeviation)
        {
            TileTypeCount = tileTypeCount;
            TileTypes = tileTypes;
            AdjacencyRules = adjacencyRules;
            Coordinates = coordinates;
            MapDimensions = mapDimensions;
            EvaluationIterations = evaluationIterations;
            KeyCeilings = keyCeilings;
            NumberOfBucketsPerAxis = numberOfBucketsPerAxis;
            StandardDeviation = standardDeviation;
        }

        public int TileTypeCount { get; }
        public List<TileType> TileTypes { get; }
        public List<AdjacencyRule> AdjacencyRules { get; }
        public List<Vector> Coordinates { get; }
        public int MapDimensions { get; }
        public int EvaluationIterations { get; }
        public KeyCeilings KeyCeilings { get; }
        public int NumberOfBucketsPerAxis { get; }
        public double StandardDeviation { get; }
    }
}