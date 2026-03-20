using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Models;
using Models;
using WFC.Models;
using WFC.Output;

namespace WFC.Extensions
{
    public static class LevelExtensions
    {
        public static void RemoveBorderTilesFromCenterOptions(this Level level)
        {
            BitArray hasNorthRules = new BitArray(level.TotalTileTypeCount, false);
            BitArray hasSouthRules = new BitArray(level.TotalTileTypeCount, false);
            BitArray hasEastRules = new BitArray(level.TotalTileTypeCount, false);
            BitArray hasWestRules = new BitArray(level.TotalTileTypeCount, false);

            for (int i = 0; i < level.Rules.Length; i++)
            {
                Dictionary<Direction, BitArray> rules = level.Rules[i].ValidTileIds;
                hasNorthRules[i] = rules[Direction.North].HasAnySetBits();
                hasSouthRules[i] = rules[Direction.South].HasAnySetBits();
                hasEastRules[i] = rules[Direction.East].HasAnySetBits();
                hasWestRules[i] = rules[Direction.West].HasAnySetBits();
            }

            BitArray[] excludedOptions = new BitArray[level.Options.Length];
            for (int i = 0; i < excludedOptions.Length; i++)
            {
                excludedOptions[i] = new BitArray(level.Options[i].Count, true);
            }
            
            for (int i = 0; i < level.NeighborIndices.Length; i++)
            {
                Dictionary<Direction, int> neighborIndices = level.NeighborIndices[i].Indices;
                if (neighborIndices.ContainsKey(Direction.North)) excludedOptions[i].And(hasNorthRules);
                if (neighborIndices.ContainsKey(Direction.South)) excludedOptions[i].And(hasSouthRules);
                if (neighborIndices.ContainsKey(Direction.East)) excludedOptions[i].And(hasEastRules);
                if (neighborIndices.ContainsKey(Direction.West)) excludedOptions[i].And(hasWestRules);
            }

            for (int i = 0; i < level.Options.Length; i++)
            {
                excludedOptions[i].Not();
                level.UpdateSumOfWeights(i, excludedOptions[i]);
                level.Options[i].Xor(excludedOptions[i]);
            }

            level.ReduceEntropyForAll(); 
        }

        internal static void BanTile(this Level level, int cellIndex, int tileIndex)
        {
            // Ban tile from options
            // Set SupportCounters to 0 for this cell and tile for all directions
            // Push onto stack
            // Update SumsOfWeights and WeightsLogWeights
            // Update Entropy
        }

        public static void ReduceEntropyForAll(this Level level)
        {
            for (int i = 0; i < level.Position.Length; i++)
            {
                ReduceEntropy(level, i);
            }
        }

        public static void ReduceEntropy(this Level level, int cellIndex)
        {
            if (level.Collapsed[cellIndex])
            {
                return;
            }

            Neighbors neighbors = level.NeighborIndices[cellIndex];
            BitArray validNeighbors = new BitArray(level.Options[cellIndex].Count, defaultValue: true);

            foreach ((Direction direction, int neighborIndex) in neighbors.Indices)
            {
                level.UpdateValidNeighbors(validNeighbors, neighborIndex, direction);
            }

            BitArray validNeighborsInCurrentOptions = validNeighbors.And(level.Options[cellIndex]);
            BitArray excludedOptions = validNeighborsInCurrentOptions.Xor(level.Options[cellIndex]);
            level.UpdateSumOfWeights(cellIndex, excludedOptions);
            level.Options[cellIndex].Xor(excludedOptions);

            level.Entropy[cellIndex] =
                CalculateEntropy(level.SumOfWeights[cellIndex], level.SumOfWeightsLogWeights[cellIndex]);
        }

        public static void UpdateValidNeighbors(this Level level, BitArray validNeighbors, int neighborIndex,
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

        public static void UpdateSumOfWeights(this Level level, int cellIndex, BitArray excludedOptions)
        {
            for (int i = 0; i < excludedOptions.Count; i++)
            {
                if (!excludedOptions[i])
                {
                    continue;
                }

                level.SumOfWeights[cellIndex] -= level.Weights[i];
                level.SumOfWeightsLogWeights[cellIndex] -= level.Weights[i] * MathF.Log(level.Weights[i], 2f);
            }
        }

        public static float CalculateEntropy(int sumOfWeights, float sumOfWeightsLogWeight)
        {
            if (sumOfWeights <= 0)
                return 0;

            return MathF.Log(sumOfWeights, 2f) - sumOfWeightsLogWeight / sumOfWeights;
        }

        public static bool IsCollapsed(this Level level)
        {
            for (var i = 0; i < level.Collapsed.Length; i++)
            {
                if (!level.Collapsed[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsCollapsed(this bool[] collapsed)
        {
            for (var i = 0; i < collapsed.Length; i++)
            {
                if (!collapsed[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsCollapsed(this BitArray collapsed)
        {
            return collapsed.HasAllSetBits();
        }

        public static bool IsFeasible(this Level level)
        {
            for (var i = 0; i < level.Options.Length; i++)
            {
                if (level.Collapsed[i])
                {
                    continue;
                }

                if (level.Options[i].HasAnySetBits())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanStep(this Level level)
        {
            for (int i = 0; i < level.Collapsed.Length; i++)
            {
                if (!level.Collapsed[i] && level.Options[i].PopCount() > 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static Map ToMap(this Level level)
        {
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                if (!level.Collapsed[i])
                    continue;

                int tileIndex = level.Options[i].GetCollapsedTileIndex();
                TileType tileType = level.TileTypes[tileIndex];
                tiles.Add(new Tile(level.Position[i].X, level.Position[i].Y, tileType.Id));
            }

            return new Map(tiles);
        }

        public static List<EmptyTile> GetEmptyTiles(this Level level)
        {
            List<EmptyTile> tiles = new List<EmptyTile>();
            for (int i = 0; i < level.Position.Length; i++)
            {
                if (level.Collapsed[i])
                    continue;

                int options = level.Options[i].PopCount();
                float entropy = level.Entropy[i];
                tiles.Add(new EmptyTile(level.Position[i].X, level.Position[i].Y, options, entropy));
            }

            return tiles;
        }
    }
}