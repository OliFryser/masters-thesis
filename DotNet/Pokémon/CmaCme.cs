using System;
using System.Collections.Generic;
using MapElites.Args;
using MapElites.Extensions;
using MapElites.Models;
using MapElites.Statistics;
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

            List<IStatisticsTracker> statisticsTrackers = args.MapElitesArgs.StatisticsTrackers;
            
            Action<string> logger = mapElitesArgs.Logger;

            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive =
                new ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(
                    individualHandler.BucketCapacity, mapElitesArgs.MapId);

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
                ScorerType.Optimization));

            emitters.AddRange(CreateEmitters(
                emitterConfiguration.FeasibilityEmitters,
                archive,
                args.StartingStepSize,
                ScorerType.Feasibility));

            emitters.AddRange(CreateEmitters(
                emitterConfiguration.RandomDirectionEmitters,
                archive,
                args.StartingStepSize,
                ScorerType.RandomDirection));

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

                Emitter currentEmitter = emitters.MinBy(e => e.GeneratedSolutions);

                if (currentEmitter.ShouldReset())
                {
                    logger("Resetting emitter");
                    var newMeanEntry = SampleEntryForEmitterType(currentEmitter.ScorerType, archive);
                    currentEmitter.Reset(newMeanEntry);
                }

                Individual individual = currentEmitter.Ask();
                ConstrainedEntry<Individual, Behavior> entry = individualHandler.Evaluate(individual);
                Key key = individualHandler.GetKey(entry.Behavior);
                bool wasSaved = archive.TryAdd(key, entry);
                statisticsTrackers.ForEach(s => s.AddPoint(archive));
                currentEmitter.Tell(entry, wasSaved);
            }

            statisticsTrackers.ForEach(s => s.SaveToFile(mapElitesArgs.StatisticsOutputPath));

            return archive;

            void EvaluateAndSave(Individual individual)
            {
                ConstrainedEntry<Individual, Behavior> entry = individualHandler.Evaluate(individual);
                Key key = individualHandler.GetKey(entry.Behavior);
                archive.TryAdd(key, entry);
                statisticsTrackers.ForEach(s => s.AddPoint(archive));
            }
        }

        private static ConstrainedEntry<Individual, Behavior> SampleEntryForEmitterType(
            ScorerType scorerType,
            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive)
        {
            switch (scorerType)
            {
                case ScorerType.Feasibility:
                {
                    if (archive.TrySampleInfeasibleIndividual(out var entry))
                        return entry;
                    return archive.SampleEntry();
                }
                case ScorerType.Optimization:
                {
                    if (archive.TrySampleFeasibleIndividual(out var entry))
                    {
                        return entry;
                    }
                    return archive.SampleEntry();
                }
                case ScorerType.RandomDirection:
                    return archive.SampleEntry();
                default:
                    throw new ArgumentOutOfRangeException(nameof(scorerType), scorerType,
                        "Entry sampling not implemented for scorer type");
            }
        }

        private static IEnumerable<Emitter> CreateEmitters(
            int amountToCreate,
            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive,
            double startingStepSize,
            ScorerType scorerType)
        {
            for (int i = 0; i < amountToCreate; i++)
            {
                ConstrainedEntry<Individual, Behavior> meanEntry = SampleEntryForEmitterType(scorerType, archive);
                yield return new Emitter(meanEntry, startingStepSize, scorerType);
            }
        }
    }
}