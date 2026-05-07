using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class FeasibilityScorer : IScorer
    {
        public double GetScore(ConstrainedEntry<Individual, Behavior> entry,
            ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            // We crossed the threshold! Big reward!
            return entry.IsFeasible ? 10 : entry.Feasibility - meanEntry.Feasibility;
        }
    }
}