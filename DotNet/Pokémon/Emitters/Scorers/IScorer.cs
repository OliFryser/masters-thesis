using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public interface IScorer
    {
        double GetScore(ConstrainedEntry<Individual, Behavior> entry, ConstrainedEntry<Individual, Behavior> meanEntry);

        void Reset()
        {
        }
    }
}