using System;

namespace Domain.Args
{
    public readonly struct KeyCeilings
    {
        public readonly float SpecialTileCeiling;
        public readonly float VariationPercentageCeiling;

        public KeyCeilings(float specialTileCeiling, float variationPercentageCeiling)
        {
            SpecialTileCeiling = specialTileCeiling;
            VariationPercentageCeiling = variationPercentageCeiling;
        }

        public override string ToString()
        {
            return $"Special Tile Percentage Ceiling: {SpecialTileCeiling}{Environment.NewLine}" +
                   $"Variation Percentage Ceiling: {VariationPercentageCeiling}";
        }
    }
}