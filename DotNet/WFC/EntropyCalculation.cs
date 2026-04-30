using System;

namespace WFC
{
    internal static class EntropyCalculation
    {
        internal static double CalculateEntropy(double sumOfWeights, double sumOfWeightsLogWeight)
        {
            if (sumOfWeights <= 0)
                return 0;

            return Math.Log(sumOfWeights, 2f) - sumOfWeightsLogWeight / sumOfWeights;
        }

        internal static double WeightLogWeight(double weight)
        {
            return weight <= 0
                ? 0f
                : weight * Math.Log(weight, 2f);
        }
    }
}