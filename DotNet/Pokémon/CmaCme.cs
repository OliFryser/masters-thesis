using System;
using System.Collections.Generic;
using MapElites.Args;
using MapElites.Extensions;
using MapElites.Models;
using Pokémon.Args;
using Pokémon.Emitters;
using Pokémon.Emitters.Scorers;

namespace Pokémon
{
    public static class CmaCme
    {
        public static ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>
            Run(CmaCmeArgs args)
        {
            MapElitesArgs mapElitesArgs = args.MapElitesArgs;
            ConstrainedIndividualHandler individualHandler = args.ConstrainedIndividualHandler;
            EmitterConfiguration emitterConfiguration = args.EmitterConfiguration;
            
            Action<string> logger = mapElitesArgs.Logger;
            
            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive = new ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(
                individualHandler.BucketCapacity);

            for (int i = 0; i < mapElitesArgs.InitializationIterations; i++)
            {
                if (i % 10 == 0)
                {
                    logger($"Completed {i} initialization iterations out of {mapElitesArgs.InitializationIterations} " +
                           $"({i / (float)mapElitesArgs.InitializationIterations * 100:F0} %). " +
                           $"Archive Size: {archive.Count}. " +
                           $"Max fitness {archive.GetMaxFitness()}");
                }

                EvaluateAndSave(individualHandler.CreateRandom());
            }

            // Initialize Emitters
            List<Emitter> emitters = new List<Emitter>();

            emitters.AddRange(CreateEmitters(
                emitterConfiguration.OptimizationEmitters,
                archive,
                args.StartingStepSize,
                () => new OptimizationScorer()));

            emitters.AddRange(CreateEmitters(
                emitterConfiguration.FeasibilityEmitters,
                archive,
                args.StartingStepSize,
                () => new FeasibilityScorer()));

            emitters.AddRange(CreateEmitters(
                emitterConfiguration.RandomDirectionEmitters,
                archive,
                args.StartingStepSize,
                () => new RandomDirectionScorer()));
            
            // Sample N individuals with emitters
            for (int i = 0; i < mapElitesArgs.MutationIterations; i++)
            {
                if (i % 10 == 0)
                {
                    logger($"Completed {i} mutation iterations out of {mapElitesArgs.MutationIterations} " +
                           $"({i / (float)mapElitesArgs.MutationIterations * 100:F0} %). " +
                           $"Archive Size: {archive.Count}. " +
                           $"Max fitness {archive.GetMaxFitness()}");
                }

                Emitter? currentEmitter = emitters.MinBy(e => e.GeneratedSolutions);

                if (currentEmitter.IsConverged)
                {
                    logger("Resetting emitter");
                    currentEmitter.Reset(archive.SampleEntry());
                }
                
                Individual individual = currentEmitter.Ask();
                ConstrainedEntry<Individual, Behavior> entry = individualHandler.Evaluate(individual);
                Key key = individualHandler.GetKey(entry.Behavior);
                archive.TryAdd(key, entry);
                currentEmitter.Tell(entry);
            }

            mapElitesArgs.StatisticsTrackers.ForEach(s => s.SaveToFile(mapElitesArgs.StatisticsOutputPath));

            return archive;

            void EvaluateAndSave(Individual individual)
            {
                ConstrainedEntry<Individual, Behavior> entry = individualHandler.Evaluate(individual);
                Key key = individualHandler.GetKey(entry.Behavior);
                archive.TryAdd(key, entry);
                mapElitesArgs.StatisticsTrackers.ForEach(s => s.AddPoint(archive));
            }
        }

        private static IEnumerable<Emitter> CreateEmitters(
            int amountToCreate,
            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive,
            double startingStepSize,
            Func<IScorer> createScorer)
        {
            for (int i = 0; i < amountToCreate; i++)
            {
                ConstrainedEntry<Individual, Behavior> meanEntry = archive.SampleEntry();
                yield return new Emitter(meanEntry, startingStepSize, createScorer());
            }
        }
    }
}