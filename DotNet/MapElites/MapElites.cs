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
                Result<TBehavior> result = populationManager.Evaluate(individual);

                TKey key = populationManager.GetKey(result.Behavior);

                Entry<TIndividual, TBehavior> entry =
                    new Entry<TIndividual, TBehavior>(individual, result.Behavior, result.Fitness);

                archive.TryAdd(key, entry);
            }
        }
    }
}