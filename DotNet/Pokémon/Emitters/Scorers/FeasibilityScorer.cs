using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class FeasibilityScorer : IScorer
    {
        private double _meanFeasibility;
        
        public void Initialize(ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            _meanFeasibility = meanEntry.Feasibility;
        }

        public double GetScore(ConstrainedEntry<Individual, Behavior> entry) => entry.Feasibility - _meanFeasibility;
    }
}