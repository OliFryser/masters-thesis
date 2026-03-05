using System;
using System.Collections.Generic;
using System.Linq;
using WFC.Args;
using WFC.Extensions;
using Models;
using WFC.Models;
using WFC.Output;

namespace WFC
{
    public static class WaveFunctionCollapse
    {
        private static readonly Random Rng = new Random();
        
        public static State Step(State state)
        {
            if (state.Level.IsComplete() || state.Level.IsInfeasible())
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
            Status status = new Status(level.IsComplete());
            return new Result(level.ToMap(), status);
        }

        private static void Step(Level level)
        {
            int cellToCollapseIndex = PickCell(level);
            CollapseCell(level, cellToCollapseIndex);
            Propagate(level, cellToCollapseIndex);
        }

        private static void Complete(Level level)
        {
            while (!level.IsComplete() && !level.IsInfeasible())
            {
                Step(level);
            }
        }

        // Pick the lowest entropy
        // Unless multiple ones have the same entropy,
        // then pick a random element among the non-collapsed ones
        private static int PickCell(Level level)
        {
            float lowestEntropy = float.PositiveInfinity;
            List<int> lowestEntropyIndices = new List<int>();
            for (int i = 0; i < level.Entropy.Length; i++)
            {
                if (level.Collapsed[i])
                {
                    continue;
                }
                
                float entropy = level.Entropy[i];
                if (Math.Abs(entropy - lowestEntropy) < 0.1f)
                {
                    lowestEntropy = entropy;
                    lowestEntropyIndices.Clear();
                    lowestEntropyIndices.Add(i);
                } 
                else if (entropy < lowestEntropy)
                {
                    lowestEntropyIndices.Add(i);
                }
            }

            return lowestEntropyIndices.GetRandomElement();
        }

        private static void CollapseCell(Level level, int cellToCollapseIndex)
        {
            int chosenTileType = level.Options[cellToCollapseIndex].GetRandomElement();
            level.Options[cellToCollapseIndex] = new HashSet<int> { chosenTileType };
            level.Collapsed[cellToCollapseIndex] = true;
        }

        private static void Propagate(Level level, int collapsedCellIndex)
        {
            foreach ((Direction _, int cellId) in level.NeighborIndices[collapsedCellIndex].Indices)
            {
                ReduceEntropy(level, cellId);
            }
        }

        private static void ReduceEntropy(Level level, int cellIndex)
        {
            if (level.Collapsed[cellIndex])
            {
                return;
            }
            
            Neighbors neighbors = level.NeighborIndices[cellIndex];
            
            foreach ((Direction direction, int neighborIndex) in neighbors.Indices)
            {
                HashSet<int> neighborOptions = level.Options[neighborIndex];
                HashSet<int> validOptions = new HashSet<int>();
                foreach (int tileId in neighborOptions)
                {
                    TileRules rules = level.Rules[tileId];
                    int[] validTiles = rules.ValidTileIds[direction.Reverse()];
                    validOptions.UnionWith(validTiles);
                }
                level.Options[cellIndex].IntersectWith(validOptions);
            }

            level.Entropy[cellIndex] = level.Options[cellIndex].Count;// + (float)Rng.NextDouble();
        }
    }
}