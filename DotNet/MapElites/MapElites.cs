using System;
using System.Collections.Generic;
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
            Archive<TKey, TEntry, TIndividual, TBehavior> archive =
                new Archive<TKey, TEntry, TIndividual, TBehavior>(individualHandler.BucketCapacity);

            return RunMapElites(archive, args, individualHandler);
        }

        public static ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>
            RunConstrained<TKey, TEntry, TIndividual, TBehavior>(IIndividualHandler<TKey, TEntry, TIndividual, TBehavior> individualHandler,
                MapElitesArgs args)
            where TKey : BaseKey<TKey>
            where TEntry : ConstrainedEntry<TIndividual, TBehavior>
        {
            ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior> archive =
                new ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>(individualHandler.BucketCapacity);

            return RunMapElites(archive, args, individualHandler);
        }

        private static TArchive RunMapElites<TArchive, TKey, TEntry, TIndividual, TBehavior>(
            TArchive archive, MapElitesArgs args,
            IIndividualHandler<TKey, TEntry, TIndividual, TBehavior> individualHandler)
            where TArchive : IArchive<TKey, TEntry, TIndividual, TBehavior>, IArchiveStatisticsProvider
            where TKey : BaseKey<TKey>
            where TEntry : Entry<TIndividual, TBehavior>
        {
            Action<string> logger = args.Logger;

            for (int i = 0; i < args.InitializationIterations; i++)
            {
                TIndividual individual = individualHandler.CreateRandom();

                EvaluateAndSave(individual);

                if (i % 10 == 0)
                {
                    logger($"Completed {i} initialization iterations out of {args.InitializationIterations} " +
                           $"({(i / (float)args.InitializationIterations) * 100:F0} %). " +
                           $"Archive Size: {archive.Count}. " +
                           $"Max fitness {archive.GetMaxFitness()}");
                }
            }
            
            for (int i = 0; i < args.MutationIterations; i++)
            {
                TIndividual individual = archive.Sample();

                TIndividual mutation = individualHandler.Mutate(individual);

                EvaluateAndSave(mutation);
                
                if (i % 10 == 0)
                {
                    logger($"Completed {i} mutation iterations out of {args.MutationIterations} " +
                           $"({(i / (float)args.MutationIterations) * 100:F0} %). " +
                           $"Archive Size: {archive.Count}. " +
                           $"Max fitness {archive.GetMaxFitness()}");
                }
            }
            
            logger($"Map-Elites completed (100%). Archive Size: {archive.Count}. Max fitness {archive.GetMaxFitness()}\n");

            args.StatisticsTrackers.ForEach(s => s.SaveToFile(args.StatisticsOutputPath));

            return archive;

            void EvaluateAndSave(TIndividual individual)
            {
                TEntry entry = individualHandler.Evaluate(individual);

                TKey key = individualHandler.GetKey(entry.Behavior);

                archive.TryAdd(key, entry);

                args.StatisticsTrackers.ForEach(s => s.AddPoint(archive));
            }
        }
    }
}