using System;

namespace Pokémon
{
    public class Behavior
    {
        public Behavior(float flowerPercentage, float tileTypesUsedPercentage)
        {
            FlowerPercentage = flowerPercentage;
            TileTypesUsedPercentage = tileTypesUsedPercentage;
        }

        public static int BehaviorCount => 2;
        public float FlowerPercentage { get; }
        public float TileTypesUsedPercentage { get; }

        public float GetDeviation(Behavior averageBehavior)
        {
            float flowerDeviation = MathF.Pow(MathF.Abs(FlowerPercentage - averageBehavior.FlowerPercentage), 2);
            float tileTypesUsedDeviation = MathF.Pow(MathF.Abs(TileTypesUsedPercentage - averageBehavior.TileTypesUsedPercentage), 2);
            
            float averageDeviation = MathF.Sqrt(flowerDeviation + tileTypesUsedDeviation) / 3;

            return averageDeviation;
        }
    }
}