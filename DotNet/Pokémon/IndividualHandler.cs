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
        private List<TileType> TileTypes { get; }
        private List<AdjacencyRule> AdjacencyRules { get; }
        private List<Vector> Coordinates { get; }
        private HashSet<TileType> DoorTiles { get; }
        private HashSet<TileType> FlowerTiles { get; }
        public int BucketCapacity { get; }

        private static int NumberOfBucketsPerAxis => 5;

        public IndividualHandler(IndividualHandlerArgs individualHandlerArgs)
        {
            TileTypeCount = individualHandlerArgs.TileTypeCount;
            TileTypes = individualHandlerArgs.TileTypes;
            AdjacencyRules = individualHandlerArgs.AdjacencyRules;
            Coordinates = individualHandlerArgs.Coordinates;
            
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

            Dictionary<TileType, int> weights = TileTypes.ToDictionary(
                keySelector: t => t,
                elementSelector: t => random.Next(TileTypeCount));

            return new Individual(weights);
        }

        public Individual Mutate(Individual individual)
        {
            Dictionary<TileType, int> newWeights = new Dictionary<TileType, int>();
            Random random = new Random();
            foreach ((TileType tileType, int weight) in individual.Weights)
            {
                int noise = random.Next(-5, 5);
                int mutatedWeight = (int)MathF.Max(weight + noise, 0);
                newWeights.Add(tileType, mutatedWeight);
            }

            return new Individual(newWeights);
        }


        public Entry Evaluate(Individual individual)
        {
            int iterations = 10;
            int amountComplete = 0;
            Behavior[] behaviors = new Behavior[iterations];

            for (int i = 0; i < iterations; i++)
            {
                WfcArgs args = new WfcArgs(Coordinates, TileTypes, AdjacencyRules, individual.Weights, i);
                State state = WaveFunctionCollapse.Run(args);

                if (i == 0)
                {
                    individual.WfcInstance = state;
                }
                
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

            return new Key(flowerBucket, doorBucket);
        }

        private Behavior GetBehavior(State state)
        {
            List<Tile> tiles = state.GetMap().Tiles;
            var numberOfFlowers = tiles.Count(t => FlowerTiles.Contains(t.Type));
            var numberOfDoors = tiles.Count(t => DoorTiles.Contains(t.Type));
            return new Behavior(
                numberOfFlowers / (float)tiles.Count,
                numberOfDoors / (float)tiles.Count);
        }

        private Behavior GetAverageBehavior(Behavior[] behaviors)
        {
            float averageFlowerPercentage = behaviors.Select(b => b.FlowerPercentage).Average();
            float averageDoorPercentage = behaviors.Select(b => b.DoorPercentage).Average();

            return new Behavior(averageFlowerPercentage, averageDoorPercentage);
        }
    }
}