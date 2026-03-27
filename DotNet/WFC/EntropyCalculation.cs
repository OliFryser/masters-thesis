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

        internal static float WeightLogWeight(int weight)
        {
            return weight <= 0
                ? 0f
                : weight * MathF.Log(weight, 2f);
        }
    }
}