using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
using Domain.Extensions;
using Domain.Models;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;

namespace WFC
{
    public static class WaveFunctionCollapse
    {
        public static State Step(State state)
        {
            if (state.Level.IsCollapsed() || !state.Level.IsFeasible())
                return state;

            Step(state.Level, state.Random);
            return state;
        }

        public static State Complete(State state)
        {
            Complete(state.Level, state.Random);
            return state;
        }

        public static State Run(in WfcArgs args)
        {
            State state = args.ToState();
            Complete(state.Level, state.Random);
            return state;
        }


        private static void Step(Level level, Random random)
        {
            var propagationStack = new UniqueStack<int>();
            int cellToCollapseIndex = PickCell(level, random);
            CollapseCell(level, cellToCollapseIndex, propagationStack, random);
            while (propagationStack.Count > 0)
            {
                int cellIndex = propagationStack.Pop();
                Propagate(level, cellIndex, propagationStack);
            }
        }

        private static void Complete(Level level, Random random)
        {
            while (!level.IsCollapsed() && level.IsFeasible())
            {
                Step(level, random);
            }
        }

        /// <summary>
        /// Picks a random cell among the lowest entropy non-collapsed cells.
        /// Assumes at least one cell has not collapsed. 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="random"></param>
        /// <returns>Index of chosen cell.</returns>
        internal static int PickCell(Level level, Random random)
        {
            double lowestEntropy = float.PositiveInfinity;
            List<int> lowestEntropyIndices = new List<int>();
            for (int i = 0; i < level.Entropy.Length; i++)
            {
                if (level.Collapsed[i] || !level.Options[i].HasAnySetBits())
                {
                    continue;
                }

                double entropy = level.Entropy[i];
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

            return lowestEntropyIndices.GetRandomElement(random);
        }

        internal static void CollapseCell(
            Level level, 
            int cellToCollapseIndex, 
            UniqueStack<int> propagationStack, 
            Random random)
        {
            BitArray oldOptions = new BitArray(level.Options[cellToCollapseIndex]);
            
            int chosenTileType =
                level.Options[cellToCollapseIndex].GetRandomWeightedSetIndex(
                    level.Weights,
                    level.SumOfWeights[cellToCollapseIndex],
                    random);
            
            level.Options[cellToCollapseIndex].SetAll(false);
            level.Options[cellToCollapseIndex][chosenTileType] = true;
            level.Collapsed[cellToCollapseIndex] = true;
            
            // Everything is known about the cell
            level.Entropy[cellToCollapseIndex] = 0f;
            
            BitArray excludedOptions = oldOptions.Xor(level.Options[cellToCollapseIndex]);
            if (excludedOptions.HasAnySetBits())
            {
                propagationStack.Push(cellToCollapseIndex);
            }
        }
        
        internal static void Propagate(Level level, int collapsedCellIndex, UniqueStack<int> propagationStack)
        {
            foreach ((Direction _, int cellIndex) in level.NeighborIndices[collapsedCellIndex].Indices)
            {
                if (level.Collapsed[cellIndex])
                {
                    continue;
                }

                BitArray excludedOptions = level.PruneInconsistentOptions(cellIndex);
                if (!excludedOptions.HasAnySetBits())
                {
                    continue;
                }

                level.UpdateSumOfWeights(cellIndex, excludedOptions);
                level.ReduceEntropy(cellIndex);
                propagationStack.Push(cellIndex);
            }
        }
    }
}