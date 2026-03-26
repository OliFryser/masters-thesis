using System;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace WFC.Extensions
{
    public static class BitArrayExtensions
    {
        #region NetStandard Compatibility Extensions
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(this BitArray bitArray)
        {
#if (NET5_0_OR_GREATER)
            int count = 0;
            int[] ints = new int[(bitArray.Count + 31) / 32];
            bitArray.CopyTo(ints, 0);

            foreach (int n in ints)
            {
                count += BitOperations.PopCount((uint)n);
            }

            return count;
#else
            return PopCountOld(bitArray);
#endif
        }

        private static int PopCountOld(this BitArray bitArray)
        {
            int count = 0;
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i]) count++;
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCollapsed(this BitArray bitArray)
        {
            return bitArray.PopCount() == 1;
        }
        
        public static bool HasAnySetBits(this BitArray bitArray)
        {
#if (NET5_0_OR_GREATER)
            return bitArray.HasAnySet();
#else
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i]) return true;
            }

            return false;
#endif
        }
        
        public static bool HasAllSetBits(this BitArray bitArray)
        {
#if (NET5_0_OR_GREATER)
            return bitArray.HasAllSet();
#else
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (!bitArray[i]) return false;
            }

            return true;
#endif
        }
        
        #endregion

        public static int GetCollapsedTileIndex(this BitArray bitArray)
        {
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i]) return i;
            }

            throw new ArgumentException("No valid tile was set as an option!");
        }
        
        public static int GetRandomWeightedSetIndex(
            this BitArray bitArray,
            int[] weights, 
            int sumOfWeights, 
            Random random)
        {
            int roll = random.Next(sumOfWeights);

            int sum = 0;
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (!bitArray[i]) 
                    continue;
                
                sum += weights[i];
                if (sum >= roll)
                {
                    return i;
                }
            }

            throw new ArgumentException("Sum of weights out of bounds or no valid set index in bitarray");
        }
    }
}