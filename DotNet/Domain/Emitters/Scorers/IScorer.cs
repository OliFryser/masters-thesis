using MapElites.Models;

namespace Domain.Emitters.Scorers
{
    public enum ScorerType
    {
        Feasibility,
        Optimization,
        RandomDirection,
    }
    
    public interface IScorer
    {
        double GetScore(ConstrainedEntry<Individual, Behavior> entry, ConstrainedEntry<Individual, Behavior> meanEntry);

        void Reset()
        {
        }
    }
}