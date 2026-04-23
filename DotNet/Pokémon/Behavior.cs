using System;

namespace Pokémon
{
    public class Behavior
    {
        public Behavior(float flowerPercentage, float doorPercentage, float tileTypesUsedPercentage)
        {
            FlowerPercentage = flowerPercentage;
            DoorPercentage = doorPercentage;
            TileTypesUsedPercentage = tileTypesUsedPercentage;
        }

        public static int BehaviorCount => 3;
        public float FlowerPercentage { get; }
        public float DoorPercentage { get; }
        public float TileTypesUsedPercentage { get; }

        public float GetDeviation(Behavior averageBehavior)
        {
            float doorDeviation = MathF.Pow(MathF.Abs(DoorPercentage - averageBehavior.DoorPercentage), 2);
            float flowerDeviation = MathF.Pow(MathF.Abs(FlowerPercentage - averageBehavior.FlowerPercentage), 2);
            float tileTypesUsedDeviation = MathF.Pow(MathF.Abs(TileTypesUsedPercentage - averageBehavior.TileTypesUsedPercentage), 2);
            
            float averageDeviation = MathF.Sqrt(doorDeviation + flowerDeviation + tileTypesUsedDeviation) / 3;

            return averageDeviation;
        }
    }
}