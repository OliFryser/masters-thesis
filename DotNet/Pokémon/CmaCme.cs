using MapElites.Args;
using MapElites.Models;
using Pokémon.Args;
using Pokémon.Emitters;
using Pokémon.Emitters.Scorers;

namespace Pokémon
{
    public static class CmaCme
    {
        public static ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> 
            Run(MapElitesArgs mapElitesArgs, ConstrainedIndividualHandler individualHandler)
        {
            var archive = new ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(
                individualHandler.BucketCapacity);

            for (int i = 0; i < mapElitesArgs.InitializationIterations; i++)
            {
                EvaluateAndSave(individualHandler.CreateRandom());
            }
            
            // Initialize Emitters
            Emitter[] emitters = 
                new[]
                {
                    new Emitter(archive.SampleEntry(), individualHandler.StandardDeviation, new OptimizationScorer()),
                    new Emitter(archive.SampleEntry(), individualHandler.StandardDeviation, new RandomDirectionScorer()),
                    new Emitter(archive.SampleEntry(), individualHandler.StandardDeviation, new FeasibilityScorer()),
                };


            return archive;
            
            void EvaluateAndSave(Individual individual)
            {
                ConstrainedEntry<Individual, Behavior> entry = individualHandler.Evaluate(individual);

                Key key = individualHandler.GetKey(entry.Behavior);

                archive.TryAdd(key, entry);

                mapElitesArgs.StatisticsTrackers.ForEach(s => s.AddPoint(archive));
            }
        }
    }
}