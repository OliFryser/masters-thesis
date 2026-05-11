using System;

namespace Pokémon.Args
{
    public readonly struct KeyCeilings
    {
        public readonly float FlowerPercentageCeiling;
        public readonly float VariationPercentageCeiling;

        public KeyCeilings(float flowerPercentageCeiling, float variationPercentageCeiling)
        {
            FlowerPercentageCeiling = flowerPercentageCeiling;
            VariationPercentageCeiling = variationPercentageCeiling;
        }

        public override string ToString()
        {
            return $"Flower Percentage Ceiling: {FlowerPercentageCeiling}{Environment.NewLine}" +
                   $"Variation Percentage Ceiling: {VariationPercentageCeiling}";
        }
    }
}