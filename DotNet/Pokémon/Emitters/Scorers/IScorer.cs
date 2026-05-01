using System.ComponentModel;
using MapElites.Models;

namespace Pokémon.Emitters.Scorers
{
    public interface IScorer
    {
        void Initialize(ConstrainedEntry<Individual, Behavior> meanEntry);
        double GetScore(ConstrainedEntry<Individual, Behavior> entry);
    }
}