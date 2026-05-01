using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public class OptimizationScorer : IScorer
    {
        private double _meanFitness;
        
        public void Initialize(ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            _meanFitness = meanEntry.Fitness;
        }

        public double GetScore(ConstrainedEntry<Individual, Behavior> entry) => entry.Fitness - _meanFitness;
    }
}