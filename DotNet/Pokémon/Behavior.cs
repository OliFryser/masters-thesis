using System;

namespace Pokémon
{
    public class Behavior
    {
        public Behavior(float specialTilePercentage, float variation)
        {
            SpecialTilePercentage = specialTilePercentage;
            Variation = variation;
        }

        public static string BehaviorXName = "Flowers %";
        public static readonly string BehaviorYName = "Variation %";
        
        public static uint BehaviorCount => 2;
        public float SpecialTilePercentage { get; }
        public float Variation { get; }

        public float GetDeviation(Behavior averageBehavior)
        {
            float specialTileDeviation = MathF.Pow(MathF.Abs(SpecialTilePercentage - averageBehavior.SpecialTilePercentage), 2);
            float variationDeviation = MathF.Pow(MathF.Abs(Variation - averageBehavior.Variation), 2);
            
            float averageDeviation = MathF.Sqrt(specialTileDeviation + variationDeviation);

            return averageDeviation;
        }
    }
}