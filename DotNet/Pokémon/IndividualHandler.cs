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
        private HashSet<TileType> DoorTiles { get; }
        private HashSet<TileType> FlowerTiles { get; }
        public int BucketCapacity { get; }
        protected int EvaluationIterations { get; }

        protected static int NumberOfBucketsPerAxis => 5;

        public IndividualHandler(IndividualHandlerArgs individualHandlerArgs)
        {
            TileTypeCount = individualHandlerArgs.TileTypeCount;
            TileTypes = individualHandlerArgs.TileTypes;
            AdjacencyRules = individualHandlerArgs.AdjacencyRules;
            Coordinates = individualHandlerArgs.Coordinates;
            EvaluationIterations = individualHandlerArgs.EvaluationIterations;
            
            DoorTiles = new HashSet<TileType>()
            {
                new TileType("8003a2e1d3f57ad878dc5ae8443ba9a1b2012142"),
                new TileType("f0e58e8686e7e54af622e5bfe3bb38953ed16430"),
            };
            
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
            foreach (TileWeight tileWeight in individual.Weights)
            {
                int noise = (random.Next(0, 2) * 2 - 1) * 500;
                int mutatedWeight = (int)MathF.Max(tileWeight.Weight + noise, 0);
                newWeights.Add(new TileWeight(tileWeight.TileType, mutatedWeight));
            }

            return new Individual(newWeights, 0);
        }

        public virtual Entry Evaluate(Individual individual)
        {
            int amountComplete = 0;
            Behavior[] behaviors = new Behavior[EvaluationIterations];

            for (int i = 0; i < EvaluationIterations; i++)
            {
                WfcArgs args = new WfcArgs(Coordinates, TileTypes, AdjacencyRules, individual.Weights, i);
                State state = WaveFunctionCollapse.Run(args);
                
                if (state.IsCollapsed)
                {
                    amountComplete++;
                }

                behaviors[i] = GetBehavior(state);
            }

            Behavior averageBehavior = GetAverageBehavior(behaviors);
            return new Entry(individual, averageBehavior, amountComplete);
        }


        public Key GetKey(Behavior behavior)
        {
            int flowerBucket = (int)MathF.Floor(behavior.FlowerPercentage * NumberOfBucketsPerAxis);
            int doorBucket = (int)MathF.Floor(behavior.DoorPercentage * NumberOfBucketsPerAxis);
            int tileTypesUsedBucket = (int)MathF.Floor(behavior.TileTypesUsedPercentage * NumberOfBucketsPerAxis);

            return new Key(flowerBucket, doorBucket, tileTypesUsedBucket);
        }

        protected Behavior GetBehavior(State state)
        {
            List<Tile> tiles = state.GetMap().Tiles;
            var numberOfFlowers = tiles.Count(t => FlowerTiles.Contains(t.Type));
            var numberOfDoors = tiles.Count(t => DoorTiles.Contains(t.Type));
            var numberOfTileTypes = tiles.Select(t => t.Type).Distinct().Count();
            return new Behavior(
                numberOfFlowers / (float)Coordinates.Count,
                numberOfDoors / (float)Coordinates.Count,
                numberOfTileTypes / (float)TileTypeCount);
        }

        protected Behavior GetAverageBehavior(Behavior[] behaviors)
        {
            float averageFlowerPercentage = behaviors.Select(b => b.FlowerPercentage).Average();
            float averageDoorPercentage = behaviors.Select(b => b.DoorPercentage).Average();
            float averageNumberOfTileTypesUsedPercentage = behaviors.Select(b => b.TileTypesUsedPercentage).Average();

            return new Behavior(averageFlowerPercentage, averageDoorPercentage, averageNumberOfTileTypesUsedPercentage);
        }
    }
}