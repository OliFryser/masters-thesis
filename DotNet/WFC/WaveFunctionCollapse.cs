using System.Collections.Generic;
using System.Linq;
using WFC.Args;
using WFC.Extensions;
using Models;
using WFC.Output;

namespace WFC
{
    public static class WaveFunctionCollapse
    {
        public static Result Run(in WfcArgs args)
        {
            Level level = args.ToLevel();

            while (!level.IsComplete() && !level.IsInfeasible())
            {
                int cellToCollapseIndex = PickCell(level);
                CollapseCell(level, cellToCollapseIndex);
                Propagate(level, cellToCollapseIndex);
            }

            Status status = new Status(level.IsComplete());
            return new Result(level.ToMap(), status);
        }

        private static int PickCell(Level level)
        {
            int lowestEntropyIndex = 0;
            float lowestEntropy = float.PositiveInfinity;
            for (int i = 0; i < level.Entropy.Length; i++)
            {
                float entropy = level.Entropy[i];
                if (entropy < lowestEntropy)
                {
                    lowestEntropy = entropy;
                    lowestEntropyIndex = i;
                }
            }
            return lowestEntropyIndex;
        }

        private static void CollapseCell(Level level, int cellToCollapseIndex)
        {
            int chosenTileType = level.Options[cellToCollapseIndex].GetRandomElement();
            level.Options[cellToCollapseIndex] = new HashSet<int> { chosenTileType };
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
            Neighbors neighbors = level.NeighborIndices[cellIndex];
            
            foreach ((Direction direction, int neighborIndex) in neighbors.Indices)
            {
                HashSet<int> options = level.Options[neighborIndex];
                foreach (int tileId in options)
                {
                    TileRules rules = level.Rules[tileId];
                    int[] validTiles = rules.ValidTileIds[direction.Reverse()];
                    level.Options[cellIndex].IntersectWith(validTiles);
                }
            }
        }
    }
}