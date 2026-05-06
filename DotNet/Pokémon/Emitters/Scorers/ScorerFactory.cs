using System;

namespace Pokémon.Emitters.Scorers
{
    public static class ScorerFactory
    {
        public static IScorer CreateScorer(ScorerType scorerType)
            => scorerType switch
            {
                ScorerType.Feasibility => new FeasibilityScorer(),
                ScorerType.Optimization => new OptimizationScorer(),
                ScorerType.RandomDirection => new RandomDirectionScorer(),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(scorerType), 
                    scorerType, 
                    "ScorerType is not implemented by factory")
            };
    }
}