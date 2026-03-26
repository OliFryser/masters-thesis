using System;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive<TIndividual, TBehavior, TKey> Run<TIndividual, TBehavior, TKey>(
            IPopulationManager<TIndividual, TBehavior, TKey> populationManager,
            int initializationIterations,
            int mutationIterations)
            where TKey : IEquatable<TKey>
        {
            Archive<TIndividual, TBehavior, TKey> archive = new Archive<TIndividual, TBehavior, TKey>();

            for (int i = 0; i < initializationIterations; i++)
            {
                TIndividual individual = populationManager.CreateRandom();

                Map(individual);
            }

            for (int i = 0; i < mutationIterations; i++)
            {
                TIndividual individual = archive.Sample();

                TIndividual mutation = populationManager.Mutate(individual);

                Map(mutation);
            }

            return archive;

            void Map(TIndividual individual)
            {
                Result<TBehavior> result = populationManager.Evaluate(individual);

                TKey key = populationManager.GetKey(result.Behavior);

                Entry<TIndividual, TBehavior> entry =
                    new Entry<TIndividual, TBehavior>(individual, result.Behavior, result.Fitness);

                archive.TryAdd(key, entry);
            }
        }
    }
}