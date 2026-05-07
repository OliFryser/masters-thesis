using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class OptimizationScorer : IScorer
    {
        public double GetScore(
            ConstrainedEntry<Individual, Behavior> entry,
            ConstrainedEntry<Individual, Behavior> meanEntry) =>
            entry.IsFeasible ? entry.Fitness - meanEntry.Fitness : -10;
    }
}