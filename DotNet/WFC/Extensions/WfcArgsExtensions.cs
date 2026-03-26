using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using WFC.Args;
using Models;
using WFC.Models;

namespace WFC.Extensions
{
    public static class WfcArgsExtensions
    {
        public static Level ToLevel(this WfcArgs args)
        {
            int numberOfCells = args.Coordinates.Count;
            int numberOfTiles = args.TileTypes.Count;

            // convert tile ids to tile indices
            TileType[] tileTypes = args.TileTypes.ToArray();
            Dictionary<TileType, int> tileTypeToIndex = new Dictionary<TileType, int>();
            for (int i = 0; i < tileTypes.Length; i++)
            {
                tileTypeToIndex.Add(tileTypes[i], i);
            }

            TileRules[] rules = new TileRules[numberOfTiles];
            PopulateRules(rules, tileTypeToIndex, args.AdjacencyRules);

            Vector[] positions = args.Coordinates.ToArray();

            BitArray[] options = new BitArray[numberOfCells];
            for (int i = 0; i < numberOfCells; i++)
            {
                options[i] = new BitArray(numberOfTiles);
                options[i].SetAll(true);
            }


            Neighbors[] neighborIndices = CreateNeighborsArray(positions);

            bool[] collapsed = new bool[numberOfCells];

            int[] weights = new int[numberOfTiles];
            foreach ((TileType tileType, int count) in args.TileTypeToCount)
            {
                int tileTypeIndex = tileTypeToIndex[tileType];
                weights[tileTypeIndex] = count;
            }

            int sumOfWeights = weights.Sum();
            int[] sumOfWeightsArray = Enumerable.Repeat(sumOfWeights, numberOfCells).ToArray();

            float sumOfWeightsLogWeights = weights.Sum(weight => weight * MathF.Log(weight, 2f));
            float[] sumOfWeightsLogWeightsArray = Enumerable.Repeat(sumOfWeightsLogWeights, numberOfCells).ToArray();

            float[] entropy =
                Enumerable.Repeat(
                    MathF.Log(sumOfWeights, 2f) - sumOfWeightsLogWeights / sumOfWeights, numberOfCells).ToArray();

            Level level = new Level(
                rules,
                tileTypes,
                positions,
                options,
                entropy,
                neighborIndices,
                collapsed,
                args.TileTypes.Count,
                weights,
                sumOfWeightsArray,
                sumOfWeightsLogWeightsArray
            );
            
            level.RemoveBorderTilesFromCenterOptions();

            return level;
        }

        public static State ToState(this WfcArgs args)
        {
            Level level = args.ToLevel();
            return new State(level, args.Seed);
        }

        internal static Neighbors[] CreateNeighborsArray(Vector[] positions)
        {
            Neighbors[] neighbors = new Neighbors[positions.Length];
            Dictionary<Vector, int> positionToIndex = 
                Enumerable.Range(0, positions.Length).ToDictionary(x => positions[x]);
            
            for (int i = 0; i < positions.Length; i++)
            {
                Vector position = positions[i];

                Dictionary<Direction, int> neighborIndices = new Dictionary<Direction, int>();

                Vector upPosition = new Vector(position.X, position.Y + 1);
                Vector downPosition = new Vector(position.X, position.Y - 1);
                Vector rightPosition = new Vector(position.X + 1, position.Y);
                Vector leftPosition = new Vector(position.X - 1, position.Y);
                
                if (positionToIndex.TryGetValue(upPosition, out int neighbor))
                {
                    neighborIndices[Direction.North] = neighbor;
                }
                if (positionToIndex.TryGetValue(downPosition, out neighbor))
                {
                    neighborIndices[Direction.South] = neighbor;
                }
                if (positionToIndex.TryGetValue(rightPosition, out neighbor))
                {
                    neighborIndices[Direction.East] = neighbor;
                }
                if (positionToIndex.TryGetValue(leftPosition, out neighbor))
                {
                    neighborIndices[Direction.West] = neighbor;
                }
                
                neighbors[i] = new Neighbors(neighborIndices);
            }

            return neighbors;
        }

        private static void PopulateRules(TileRules[] rules, Dictionary<TileType, int> typeToIndex,
            IReadOnlyCollection<AdjacencyRule> argsAdjacencyRules)
        {
            int numberOfTiles = typeToIndex.Count;

            Dictionary<Direction, BitArray>[]
                adjacencyRules = new Dictionary<Direction, BitArray>[numberOfTiles];

            for (int i = 0; i < rules.Length; i++)
            {
                adjacencyRules[i] = new Dictionary<Direction, BitArray>
                {
                    { Direction.North, new BitArray(numberOfTiles) },
                    { Direction.East, new BitArray(numberOfTiles) },
                    { Direction.South, new BitArray(numberOfTiles) },
                    { Direction.West, new BitArray(numberOfTiles) },
                };
            }

            foreach (AdjacencyRule adjacencyRule in argsAdjacencyRules)
            {
                int fromIndex = typeToIndex[adjacencyRule.From];
                int toIndex = typeToIndex[adjacencyRule.To];
                adjacencyRules[fromIndex][adjacencyRule.Direction][toIndex] = true;
            }

            for (int i = 0; i < rules.Length; i++)
            {
                rules[i] = new TileRules(adjacencyRules[i]);
            }
        }
    }
}