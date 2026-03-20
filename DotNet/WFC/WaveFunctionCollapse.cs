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
                level.ReduceEntropy(cellId);
                Propagate(level, cellId, depth - 1, visited);
            }
        }
    }
}