using System;
using MapElites.Args;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive<TKey, TEntry, TIndividual, TBehavior> Run<TKey, TEntry, TIndividual, TBehavior>(
            IIndividualHandler<TKey, TEntry, TIndividual, TBehavior> individualHandler,
            MapElitesArgs args)
            where TKey : BaseKey<TKey>
            where TEntry : Entry<TIndividual, TBehavior>
        {
            Archive<TKey, TEntry, TIndividual, TBehavior> archive = new Archive<TKey, TEntry, TIndividual, TBehavior>();
            Action<string> logger = args.Logger;

            for (int i = 0; i < args.InitializationIterations; i++)
            {
                TIndividual individual = individualHandler.CreateRandom();

                EvaluateAndSave(individual);

                logger($"Initialization Iterations: {i}");
                logger($"Archive size: {archive.Count}");
                logger($"Max Fitness: {archive.GetMaxFitness()}\n");
            }

            logger($"Archive Initialized. Archive Size: {archive.Count}\n");

            for (int i = 0; i < args.MutationIterations; i++)
            {
                TIndividual individual = archive.SampleRandom();

                TIndividual mutation = individualHandler.Mutate(individual);

                EvaluateAndSave(mutation);

                logger($"Mutation Iterations: {i}");
                logger($"Archive size: {archive.Count}");
                logger($"Max Fitness: {archive.GetMaxFitness()}\n");
            }

            return archive;

            void EvaluateAndSave(TIndividual individual)
            {
                TEntry entry = individualHandler.Evaluate(individual);

                TKey key = individualHandler.GetKey(entry.Behavior);

                archive.TryAdd(key, entry);
            }
        }
    }
}