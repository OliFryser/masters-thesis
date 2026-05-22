using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Domain.Args
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
            double standardDeviation,
            VariationBehavior variationBehavior,
            ProblemDomain problemDomain,
            MutationStrategy mutationStrategy)
        {
            List<Vector> coordinates =
                LevelGeneration.GetRectangleCoordinates(width: mapDimensions, height: mapDimensions).ToList();

            return new IndividualHandlerArgs(
                tileTypeCount: tileTypeCount, 
                tileTypes: tileTypes, 
                adjacencyRules: adjacencyRules, 
                coordinates: coordinates,
                mapDimensions: mapDimensions, 
                evaluationIterations: evaluationIterations, 
                keyCeilings: keyCeilings, 
                numberOfBucketsPerAxis: numberOfBucketsPerAxis, 
                standardDeviation: standardDeviation,
                variationBehavior: variationBehavior, 
                problemDomain: problemDomain, 
                mutationStrategy: mutationStrategy);
        }

        private IndividualHandlerArgs(
            int tileTypeCount,
            List<TileType> tileTypes,
            List<AdjacencyRule> adjacencyRules,
            List<Vector> coordinates,
            int mapDimensions,
            int evaluationIterations,
            KeyCeilings keyCeilings,
            int numberOfBucketsPerAxis,
            double standardDeviation,
            VariationBehavior variationBehavior,
            ProblemDomain problemDomain, 
            MutationStrategy mutationStrategy)
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
            VariationBehavior = variationBehavior;
            ProblemDomain = problemDomain;
            MutationStrategy = mutationStrategy;
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
        public VariationBehavior VariationBehavior { get; }
        public ProblemDomain ProblemDomain { get; }
        public MutationStrategy MutationStrategy { get; }
    }
}
