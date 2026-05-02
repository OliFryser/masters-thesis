using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class FeasibilityScorer : IScorer
    {
        public double GetScore(ConstrainedEntry<Individual, Behavior> entry, ConstrainedEntry<Individual, Behavior> meanEntry) =>
            entry.Feasibility - meanEntry.Feasibility;
    }
}