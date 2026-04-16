using System;
using System.Collections.Generic;
using MapElites.Args;
using MapElites.Models;
using MapElites.Statistics;

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
            List<IStatisticsTracker<TKey, TEntry, TIndividual, TBehavior>> statisticsTrackers =
                new List<IStatisticsTracker<TKey, TEntry, TIndividual, TBehavior>>
                {
                    new FitnessTracker<TKey, TEntry, TIndividual, TBehavior>(),
                    new CoverageTracker<TKey, TEntry, TIndividual, TBehavior>(),
                };
            
            Archive<TKey, TEntry, TIndividual, TBehavior> archive = 
                new Archive<TKey, TEntry, TIndividual, TBehavior>(individualHandler.BucketCapacity);
            Action<string> logger = args.Logger;

            for (int i = 0; i < args.InitializationIterations; i++)
            {
                TIndividual individual = individualHandler.CreateRandom();

                EvaluateAndSave(individual);
            }

            logger($"Archive Initialized. Archive Size: {archive.Count}\n");

            for (int i = 0; i < args.MutationIterations; i++)
            {
                TIndividual individual = archive.SampleRandom();

                TIndividual mutation = individualHandler.Mutate(individual);

                EvaluateAndSave(mutation);
            }

            statisticsTrackers.ForEach(s => s.SaveToFile(args.StatisticsOutputPath));

            return archive;

            void EvaluateAndSave(TIndividual individual)
            {
                TEntry entry = individualHandler.Evaluate(individual);

                TKey key = individualHandler.GetKey(entry.Behavior);

                archive.TryAdd(key, entry);
                
                statisticsTrackers.ForEach(s => s.AddPoint(archive));
            }
        }
    }
}