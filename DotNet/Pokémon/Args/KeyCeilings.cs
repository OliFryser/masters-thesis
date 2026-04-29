using System;

namespace Pokémon.Args
{
    public readonly struct KeyCeilings
    {
        public readonly float FlowerPercentageCeiling;
        public readonly float DoorPercentageCeiling;
        public readonly float VariationPercentageCeiling;

        public KeyCeilings(float flowerPercentageCeiling, float doorPercentageCeiling, float variationPercentageCeiling)
        {
            FlowerPercentageCeiling = flowerPercentageCeiling;
            DoorPercentageCeiling = doorPercentageCeiling;
            VariationPercentageCeiling = variationPercentageCeiling;
        }

        public override string ToString()
        {
            return $"Flower Percentage Ceiling: {FlowerPercentageCeiling}{Environment.NewLine}" +
                   $"Door Percentage Ceiling: {DoorPercentageCeiling}{Environment.NewLine}" +
                   $"Variation Percentage Ceiling: {VariationPercentageCeiling}";
        }
    }
}