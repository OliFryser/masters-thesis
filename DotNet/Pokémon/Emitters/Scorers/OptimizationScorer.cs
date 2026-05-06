using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class OptimizationScorer : IScorer
    {
        public double GetScore(
            ConstrainedEntry<Individual, Behavior> entry,
            ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            if (!entry.IsFeasible)
            {
                return -10;
            }
            return entry.Fitness - meanEntry.Fitness;
        }
    }
}