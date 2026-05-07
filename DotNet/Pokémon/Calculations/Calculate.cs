using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;

namespace Pokémon.Calculations
{
    public static class Calculate
    {
        public static float Entropy(List<Tile> tiles, int tileTypeCount)
        {
            float shannonEntropy = tiles
                .GroupBy(tile => tile.Type.Id)
                .Select(grouping =>
                {
                    float count = grouping.Count();
                    float p = count / tiles.Count;

                    return -p * MathF.Log(p, 2);
                })
                .Sum();
            
            float maxEntropy = MathF.Log(tileTypeCount, 2);

            float variation = shannonEntropy / maxEntropy;
            
            return variation;
        }

        public static float UniquePercentage(List<Tile> tiles, int tileTypeCount)
        {
            return tiles.GroupBy(tile => tile.Type.Id).Count() / (float)tileTypeCount;
        }
    }
}