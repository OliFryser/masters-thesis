using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Models;
using Models;
using WFC.Models;
using WFC.Output;
using static WFC.EntropyCalculation;

namespace WFC.Extensions
{
    internal static class LevelExtensions
    {
        internal static void RemoveBorderTilesFromCenterOptions(this Level level)
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

        private static void ReduceEntropyForAll(this Level level)
        {
            for (int i = 0; i < level.Position.Length; i++)
            {
                level.ReduceEntropy(i);
            }
        }

        internal static BitArray PruneInconsistentOptions(this Level level, int cellIndex)
        {
            if (level.Collapsed[cellIndex])
                return new BitArray(level.TotalTileTypeCount);

            BitArray accumulatedValidTileNeighbors = GetValidTilesBasedOnNeighbors(level, cellIndex);
            BitArray validNeighborsInCurrentOptions = accumulatedValidTileNeighbors.And(level.Options[cellIndex]);
            BitArray excludedOptions = validNeighborsInCurrentOptions.Xor(level.Options[cellIndex]);
            level.Options[cellIndex].Xor(excludedOptions);
            return excludedOptions;
        }

        internal static void ReduceEntropy(this Level level, int cellIndex)
        {
            if (level.Collapsed[cellIndex])
            {
                return;
            }
            
            level.Entropy[cellIndex] =
                CalculateEntropy(level.SumOfWeights[cellIndex], level.SumOfWeightsLogWeights[cellIndex]);
        }

        private static BitArray GetValidTilesBasedOnNeighbors(Level level, int cellIndex)
        {
            Neighbors neighbors = level.NeighborIndices[cellIndex];
            BitArray accumulatedValidTiles = new BitArray(level.Options[cellIndex].Count, defaultValue: true);

            foreach ((Direction direction, int neighborIndex) in neighbors.Indices)
            {
                level.AccumulateValidTilesInDirection(accumulatedValidTiles, neighborIndex, direction);
            }

            return accumulatedValidTiles;
        }

        internal static void AccumulateValidTilesInDirection(this Level level, BitArray validNeighbors, int neighborIndex,
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

            validNeighbors.And(validNeighborsInDirection);
        }

        internal static void UpdateSumOfWeights(this Level level, int cellIndex, BitArray excludedOptions)
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

        internal static bool IsCollapsed(this Level level)
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

        internal static bool IsFeasible(this Level level)
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

        internal static bool CanStep(this Level level)
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

        internal static Map ToMap(this Level level)
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

        internal static List<EmptyTile> GetEmptyTiles(this Level level)
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