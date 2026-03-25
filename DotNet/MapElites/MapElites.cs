using System;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive<TIndividual, TBehavior, TKey> Run<TIndividual, TBehavior, TKey>
        (IPopulationManager<TIndividual, TBehavior, TKey> populationManager, int initializationIterations,
            int mutationIterations) where TKey : IEquatable<TKey>
        {
            Archive<TIndividual, TBehavior, TKey> archive = new Archive<TIndividual, TBehavior, TKey>();

            for (int i = 0; i < initializationIterations; i++)
            {
                TIndividual individual = populationManager.CreateRandom();
                Result<TBehavior> result = populationManager.Evaluate(individual);
                TKey key = populationManager.GetKey(result.Behavior);
                Entry<TIndividual, TBehavior> entry =
                    new Entry<TIndividual, TBehavior>(individual, result.Behavior, result.Fitness);
                archive.TryAdd(key, entry);
            }

            for (int i = 0; i < mutationIterations; i++)
            {
                TIndividual individual = archive.Sample();
                TIndividual mutation = populationManager.Mutate(individual);
                Result<TBehavior> result = populationManager.Evaluate(mutation);
                TKey key = populationManager.GetKey(result.Behavior);
                Entry<TIndividual, TBehavior> entry =
                    new Entry<TIndividual, TBehavior>(mutation, result.Behavior, result.Fitness);
                archive.TryAdd(key, entry);
            }

            return archive;
        }
    }
}