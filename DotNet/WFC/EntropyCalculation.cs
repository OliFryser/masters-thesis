using System;

namespace WFC
{
    internal static class EntropyCalculation
    {
        internal static float CalculateEntropy(int sumOfWeights, float sumOfWeightsLogWeight)
        {
            if (sumOfWeights <= 0)
                return 0;

            return MathF.Log(sumOfWeights, 2f) - sumOfWeightsLogWeight / sumOfWeights;
        }
    }
}