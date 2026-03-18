using System;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace WFC.Extensions
{
    public static class BitArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PopCount(this BitArray bitArray)
        {
            int count = 0;
            // Copy BitArray to an int array
            int[] ints = new int[(bitArray.Count + 31) / 32];
            bitArray.CopyTo(ints, 0);

            // Use hardware-accelerated PopCount
            foreach (int n in ints)
            {
                count += BitOperations.PopCount((uint)n);
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasCollapsed(this BitArray bitArray)
        {
            return bitArray.PopCount() == 1;
        }

        public static int GetCollapsedTileIndex(this BitArray bitArray)
        {
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i]) return i;
            }

            throw new ArgumentException("No valid tile was set as an option!");
        }

        public static int GetRandomSetIndex(this BitArray bitArray)
        {
            Random rng = new Random();
            int result = -1;
            int count = 0;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    count++;
                    // With probability 1/count, replace the current result
                    if (rng.Next(count) == 0)
                    {
                        result = i;
                    }
                }
            }

            return result; // Returns -1 if no set bits were found
        }
    }
}