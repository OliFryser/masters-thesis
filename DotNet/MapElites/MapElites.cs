using System;
using MapElites.Args;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive<TKey, TEntry, TIndividual, TBehavior> Run<TKey, TEntry, TIndividual, TBehavior>(
            IPopulationManager<TKey, TEntry, TIndividual, TBehavior> populationManager,
            MapElitesArgs args)
            where TKey : IEquatable<TKey> 
            where TEntry : Entry<TIndividual, TBehavior>
        {
            Archive<TKey, TEntry, TIndividual, TBehavior> archive = new Archive<TKey, TEntry, TIndividual, TBehavior>();
            Action<string> logger = args.Logger;

            for (int i = 0; i < args.InitializationIterations; i++)
            {
                TIndividual individual = populationManager.CreateRandom();

                EvaluateAndSave(individual);
            }
            logger("Archive Initialized. Archive Size: " + archive.Count);
            
            for (int i = 0; i < args.MutationIterations; i++)
            {
                if (i % 10 == 0)
                {
                    logger("Mutation Iterations: " + i);
                    logger("Archive size: " + archive.Count);
                    logger("Max Fitness: " + archive.GetMaxFitness());
                }
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