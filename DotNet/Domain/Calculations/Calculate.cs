using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Domain.Calculations
{
    public static class Calculate
    {
        // Inspired by https://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
        public static int IntPow(this int x, uint pow)
        {
            int ret = 1;
            while ( pow != 0 )
            {
                if ( (pow & 1) == 1 )
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
        
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