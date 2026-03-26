using System;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive<TKey, TEntry, TIndividual, TBehavior> Run<TKey, TEntry, TIndividual, TBehavior>(
            IPopulationManager<TKey, TEntry, TIndividual, TBehavior> populationManager,
            int initializationIterations,
            int mutationIterations)
            where TKey : IEquatable<TKey> 
            where TEntry : Entry<TIndividual, TBehavior>, new()
        {
            Archive<TKey, TEntry, TIndividual, TBehavior> archive = new Archive<TKey, TEntry, TIndividual, TBehavior>();

            for (int i = 0; i < initializationIterations; i++)
            {
                TIndividual individual = populationManager.CreateRandom();

                EvaluateAndSave(individual);
            }

            for (int i = 0; i < mutationIterations; i++)
            {
                TIndividual individual = archive.SampleRandom();

                TIndividual mutation = populationManager.Mutate(individual);

                EvaluateAndSave(mutation);
            }

            return archive;

            void EvaluateAndSave(TIndividual individual)
            {
                TEntry entry = populationManager.Evaluate(individual);

                TKey key = populationManager.GetKey(entry.Behavior);

                // individual, result.Behavior, result.Fitness
                // TEntry entry = new TEntry();
                // Entry<TIndividual, TBehavior> entry = new Entry<TIndividual, TBehavior>();

                archive.TryAdd(key, entry);
            }
        }
    }
}