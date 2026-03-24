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
            Stack<StackEntry> propagationStack = new Stack<StackEntry>();
            int cellToCollapseIndex = PickCell(level);
            CollapseCell(level, cellToCollapseIndex, propagationStack);
            Propagate(level, propagationStack);
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
        internal static int PickCell(Level level)
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

        internal static void CollapseCell(Level level, int cellToCollapseIndex, Stack<StackEntry> propagationStack)
        {
            int chosenTileType =
                level.Options[cellToCollapseIndex].GetRandomWeightedSetIndex(
                    level.Weights,
                    level.SumOfWeights[cellToCollapseIndex]);
            // TODO call BanTile for each option set to false
            level.Options[cellToCollapseIndex].SetAll(false);
            level.Options[cellToCollapseIndex][chosenTileType] = true;
            level.Collapsed[cellToCollapseIndex] = true;
        }

        internal static void Propagate(Level level, Stack<StackEntry> stack)
        {
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var fromCellIndex = current.FromCellIndex;
                var bannedTileIndex = current.BannedTileIndex;
                foreach ((Direction direction, int cellIndex) in level.NeighborIndices[fromCellIndex].Indices)
                {
                    level.UpdateConstraints(cellIndex, fromCellIndex, bannedTileIndex, direction, stack);
                }
            }
 

        }
    }
}