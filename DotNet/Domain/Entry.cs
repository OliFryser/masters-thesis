using MapElites.Models;

namespace Domain
{
    public class Entry : Entry<Individual, Behavior>
    {
        public Entry(Individual individual, Behavior behavior, float fitness) : base(individual, behavior, fitness)
        {
        }
    }
}