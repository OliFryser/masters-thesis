using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using MapElites;
using Pokémon.Args;
using WFC;
using WFC.Args;
using WFC.Models;

namespace Pokémon
{
    public class IndividualHandler : IIndividualHandler<Key, Entry, Individual, Behavior>
    {
        private int TileTypeCount { get; }
        protected List<TileType> TileTypes { get; }
        protected List<AdjacencyRule> AdjacencyRules { get; }
        protected List<Vector> Coordinates { get; }
        private HashSet<TileType> FlowerTiles { get; }
        public int BucketCapacity { get; }
        protected int EvaluationIterations { get; }
        private KeyCeilings KeyCeilings { get; }

        public const int NumberOfBucketsPerAxis = 5;

        public IndividualHandler(IndividualHandlerArgs individualHandlerArgs)
        {
            TileTypeCount = individualHandlerArgs.TileTypeCount;
            TileTypes = individualHandlerArgs.TileTypes;
            AdjacencyRules = individualHandlerArgs.AdjacencyRules;
            Coordinates = individualHandlerArgs.Coordinates;
            EvaluationIterations = individualHandlerArgs.EvaluationIterations;
            KeyCeilings = individualHandlerArgs.KeyCeilings;

            FlowerTiles = new HashSet<TileType>()
            {
                new TileType("99907823a2961b44c2245d44f84bed3452b86f02"),
            };

            BucketCapacity = Behavior.BehaviorCount * NumberOfBucketsPerAxis;
        }

        public Individual CreateRandom()
        {
            Random random = new Random();

            // Technically, it would be more correct to use the total tile count from the
            // input image, but does it matter, since the weights are relative?
            List<TileWeight> weights = TileTypes.Select(t => new TileWeight(t, random.Next(TileTypeCount))).ToList();

            return new Individual(weights, 0);
        }

        public Individual Mutate(Individual individual)
        {
            List<TileWeight> newWeights = new List<TileWeight>();
            Random random = new Random();
            NormalSampler normalSampler = new NormalSampler();
            foreach (TileWeight tileWeight in individual.Weights)
            {
                float standardDeviation = 25f;
                double sampledWeight = normalSampler.Sample(tileWeight.Weight, standardDeviation, random);
                int roundedWeight = (int)Math.Round(sampledWeight);
                int clampedWeight = Math.Max(0, roundedWeight);
                newWeights.Add(new TileWeight(tileWeight.TileType, clampedWeight));
            }

            return new Individual(newWeights, 0);
        }

        public virtual Entry Evaluate(Individual individual)
        {
            State[] results = SampleStates(individual);
            
            float fitness = results.Count(state => state.IsCollapsed);
            
            Behavior[] behaviors = results.Select(GetBehavior).ToArray();

            Behavior averageBehavior = GetAverageBehavior(behaviors);
            
            return new Entry(individual, averageBehavior, fitness);
        }

        protected State[] SampleStates(Individual individual)
        {
            return Enumerable.Range(0, EvaluationIterations)
                .AsParallel()
                .Select(i =>
                {
                    WfcArgs args = new WfcArgs(Coordinates, TileTypes, AdjacencyRules, individual.Weights, i);
                    return WaveFunctionCollapse.Run(args);
                })
                .ToArray();
        }


        public Key GetKey(Behavior behavior)
        {
            int flowerBucket = GetBucket(behavior.FlowerPercentage, KeyCeilings.FlowerPercentageCeiling);

            int tileTypesUsedBucket =
                GetBucket(behavior.TileTypesUsedPercentage, KeyCeilings.VariationPercentageCeiling);

            return new Key(flowerBucket, tileTypesUsedBucket);
        }

        private static int GetBucket(float percentage, float percentageCeiling)
        {
            return (int)MathF.Floor(percentage / percentageCeiling * NumberOfBucketsPerAxis);
        }

        protected Behavior GetBehavior(State state)
        {
            List<Tile> tiles = state.GetMap().Tiles;
            var numberOfFlowers = tiles.Count(t => FlowerTiles.Contains(t.Type));
            var numberOfTileTypes = tiles.Select(t => t.Type).Distinct().Count();
            return new Behavior(
                numberOfFlowers / (float)Coordinates.Count,
                numberOfTileTypes / (float)TileTypeCount);
        }

        protected Behavior GetAverageBehavior(Behavior[] behaviors)
        {
            float averageFlowerPercentage = behaviors.Select(b => b.FlowerPercentage).Average();
            float averageNumberOfTileTypesUsedPercentage = behaviors.Select(b => b.TileTypesUsedPercentage).Average();

            return new Behavior(averageFlowerPercentage, averageNumberOfTileTypesUsedPercentage);
        }
    }
}