using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class FeasibilityScorer : IScorer
    {
        public double GetScore(ConstrainedEntry<Individual, Behavior> entry,
            ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            // We crossed the threshold! Big reward!
            if (entry.IsFeasible)
            {
                return 10;
            }
            return entry.Feasibility - meanEntry.Feasibility;
        }
    }
}