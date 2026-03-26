using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using MapElites;
using TilemapAnalysis;
using WFC;
using WFC.Args;
using WFC.Models;

namespace Pokémon
{
    public class PopulationManager : IPopulationManager<Key, Entry, Individual, Behavior>
    {
        private int TileCount { get; }
        private List<TileType> TileTypes { get; }
        private List<AdjacencyRule> AdjacencyRules { get; }
        private HashSet<TileType> DoorTiles { get; }
        private HashSet<TileType> FlowerTiles { get; }
        
        public PopulationManager(string inputTilemapPath)
        {
            using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(inputTilemapPath);
            TileCount = tilemapAnalyzer.TileCount;
            TileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToList();
            AdjacencyRules = tilemapAnalyzer.GetAdjacencyRules();
            DoorTiles = new HashSet<TileType>()
            {
                new TileType("-744959885"),
                new TileType("666136432"),
            };
            FlowerTiles = new HashSet<TileType>()
            {
                new TileType("2146349780"),
            };
        }

        public Individual CreateRandom()
        {
            Dictionary<TileType, int> weights = new Dictionary<TileType, int>();
            Random random = new Random();
            for (int i = 0; i < TileCount; i++)
            {
                int weight = random.Next(TileCount);
                TileType tileType = TileTypes[i];
                weights.Add(tileType, weight);
            }

            return new Individual(weights);
        }

        public Individual Mutate(Individual individual)
        {
            Dictionary<TileType, int> newWeights = new Dictionary<TileType, int>();
            Random random = new Random();
            foreach ((TileType tileType, int weight) in individual.Weights)
            {
                int noise = random.Next(-5, 5);
                newWeights.Add(tileType, weight + noise);
            }
            return new Individual(newWeights);
        }

        public Entry Evaluate(Individual individual)
        {
            int iterations = 100;
            List<Vector> positions = GetPositions(20, 20).ToList();
            int amountComplete = 0;
            Behavior[] behaviors = new Behavior[iterations];
            
            for (int i = 0; i < iterations; i++)
            {
                WfcArgs args = new WfcArgs(positions, TileTypes, AdjacencyRules, individual.Weights, i);
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
            int numberOfBuckets = 5;
            int flowerBucket = (int) MathF.Floor(behavior.FlowerPercentage * numberOfBuckets);
            int doorBucket = (int) MathF.Floor(behavior.DoorPercentage * numberOfBuckets);
            
            return new Key(flowerBucket, doorBucket);
        }
        
        private Behavior GetBehavior(State state)
        {
            List<Tile> tiles = state.GetMap().Tiles;
            var numberOfFlowers = tiles.Count(t => FlowerTiles.Contains(t.Type));
            var numberOfDoors = tiles.Count(t => DoorTiles.Contains(t.Type));
            return new Behavior(
                numberOfFlowers / (float) TileCount, 
                numberOfDoors / (float) TileCount);
        }

        private Behavior GetAverageBehavior(Behavior[] behaviors)
        {
            float averageFlowerPercentage = behaviors.Select(b => b.FlowerPercentage).Average();
            float averageDoorPercentage = behaviors.Select(b => b.DoorPercentage).Average();
            
            return new Behavior(averageFlowerPercentage, averageDoorPercentage);
        }
        
        private IEnumerable<Vector> GetPositions(int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return new Vector(x, y);
                }
            }
        }
    }
}