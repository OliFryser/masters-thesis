using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Models;
using WFC.Args;
using WFC.Extensions;
using Models;
using WFC.Models;
using WFC.Output;

namespace WFC
{
    public static class WaveFunctionCollapse
    {
        public static State Step(State state)
        {
            if (state.Level.IsCollapsed() || !state.Level.IsFeasible())
                return state;

            Step(state.Level);
            return state;
        }

        public static State Complete(State state)
        {
            Complete(state.Level);
            return state;
        }

        public static Result Run(in WfcArgs args)
        {
            Level level = args.ToLevel();
            Complete(level);
            Status status = new Status(level.IsCollapsed());
            return new Result(level.ToMap(), status);
        }

        private static void Step(Level level)
        {
            int cellToCollapseIndex = PickCell(level);
            CollapseCell(level, cellToCollapseIndex);
            Propagate(level, cellToCollapseIndex, level.MaxDepth, new HashSet<int>());
        }

        private static void Complete(Level level)
        {
            while (!level.IsCollapsed() && level.IsFeasible())
            {
                Step(level);
            }
        }

        /// <summary>
        /// Picks a random cell among the lowest entropy non-collapsed cells.
        /// Assumes at least one cell has not collapsed. 
        /// </summary>
        /// <param name="level"></param>
        /// <returns>Index of chosen cell.</returns>
        private static int PickCell(Level level)
        {
            float lowestEntropy = float.PositiveInfinity;
            List<int> lowestEntropyIndices = new List<int>();
            for (int i = 0; i < level.Entropy.Length; i++)
            {
                if (level.Collapsed[i] || !level.Options[i].HasAnySetBits())
                {
                    continue;
                }

                float entropy = level.Entropy[i];
                const float threshold = 0.1f;
                bool amongLowestEntropies = Math.Abs(entropy - lowestEntropy) < threshold;
                if (amongLowestEntropies)
                {
                    lowestEntropyIndices.Add(i);
                }
                else if (entropy < lowestEntropy)
                {
                    lowestEntropy = entropy;
                    lowestEntropyIndices.Clear();
                    lowestEntropyIndices.Add(i);
                }
            }

            return lowestEntropyIndices.GetRandomElement();
        }

        private static void CollapseCell(Level level, int cellToCollapseIndex)
        {
            int chosenTileType =
                level.Options[cellToCollapseIndex].GetRandomWeightedSetIndex(
                    level.Weights,
                    level.SumOfWeights[cellToCollapseIndex]);
            level.Options[cellToCollapseIndex].SetAll(false);
            level.Options[cellToCollapseIndex][chosenTileType] = true;
            level.Collapsed[cellToCollapseIndex] = true;
        }

        private static void Propagate(Level level, int collapsedCellIndex, int depth, HashSet<int> visited)
        {
            if (depth <= 0)
            {
                return;
            }

            if (visited.Contains(collapsedCellIndex))
            {
                return;
            }

            visited.Add(collapsedCellIndex);

            foreach ((Direction _, int cellId) in level.NeighborIndices[collapsedCellIndex].Indices)
            {
                ReduceEntropy(level, cellId);
                Propagate(level, cellId, depth - 1, visited);
            }
        }

        private static void ReduceEntropy(Level level, int cellIndex)
        {
            if (level.Collapsed[cellIndex])
            {
                return;
            }

            Neighbors neighbors = level.NeighborIndices[cellIndex];
            BitArray validNeighbors = new BitArray(level.Options[cellIndex].Count, defaultValue: true);

            foreach ((Direction direction, int neighborIndex) in neighbors.Indices)
            {
                UpdateValidNeighbors(validNeighbors, level, neighborIndex, direction);
            }

            BitArray validNeighborsInCurrentOptions = validNeighbors.And(level.Options[cellIndex]);
            BitArray excludedOptions = validNeighborsInCurrentOptions.Xor(level.Options[cellIndex]);
            UpdateSumOfWeights(level, cellIndex, excludedOptions);
            level.Options[cellIndex].Xor(excludedOptions);

            level.Entropy[cellIndex] =
                CalculateEntropy(level.SumOfWeights[cellIndex], level.SumOfWeightsLogWeights[cellIndex]);
        }

        private static void UpdateValidNeighbors(BitArray validNeighbors, Level level, int neighborIndex,
            Direction direction)
        {
            BitArray neighborOptions = level.Options[neighborIndex];
            BitArray validNeighborsInDirection = new BitArray(level.Options[neighborIndex].Count, false);

            for (int i = 0; i < neighborOptions.Count; i++)
            {
                if (neighborOptions[i])
                {
                    TileRules rules = level.Rules[i];
                    BitArray validTiles = rules.ValidTileIds[direction.Reverse()];
                    validNeighborsInDirection.Or(validTiles);
                }
            }

            if (validNeighborsInDirection.HasAnySetBits())
            {
                validNeighbors.And(validNeighborsInDirection);
            }
        }

        private static void UpdateSumOfWeights(Level level, int cellIndex, BitArray excludedOptions)
        {
            for (var i = 0; i < excludedOptions.Count; i++)
            {
                if (!excludedOptions[i])
                {
                    continue;
                }

                level.SumOfWeights[cellIndex] -= level.Weights[i];
                level.SumOfWeightsLogWeights[cellIndex] -= level.Weights[i] * MathF.Log(level.Weights[i], 2f);
            }
        }

        private static float CalculateEntropy(int sumOfWeights, float sumOfWeightsLogWeight)
        {
            if (sumOfWeights <= 0)
                return 0;

            return MathF.Log(sumOfWeights, 2f) - sumOfWeightsLogWeight / sumOfWeights;
        }
    }
}