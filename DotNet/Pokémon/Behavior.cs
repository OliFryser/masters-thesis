using System;

namespace Pokémon
{
    public class Behavior
    {
        public Behavior(float flowerPercentage, float variation)
        {
            FlowerPercentage = flowerPercentage;
            Variation = variation;
        }

        public static int BehaviorCount => 2;
        public float FlowerPercentage { get; }
        public float Variation { get; }

        public float GetDeviation(Behavior averageBehavior)
        {
            float flowerDeviation = MathF.Pow(MathF.Abs(FlowerPercentage - averageBehavior.FlowerPercentage), 2);
            float variationDeviation = MathF.Pow(MathF.Abs(Variation - averageBehavior.Variation), 2);
            
            float averageDeviation = MathF.Sqrt(flowerDeviation + variationDeviation);

            return averageDeviation;
        }
    }
}