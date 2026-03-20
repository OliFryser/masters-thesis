using System;
using MapElites.Extensions;
using MapElites.Models;

namespace MapElites
{
    public static class MapElites
    {
        public static Archive<TIndividual> Run<TIndividual>(
            Func<TIndividual> individualFactory,
            Func<TIndividual, TIndividual> individualMutator,
            Func<TIndividual, Fitness> fitnessEvaluator,
            Func<TIndividual, Behavior> behaviorEvaluator)
        {
            const int initializationIterations = 3;
            const int iterations = 10;

            Archive<TIndividual> archive = new Archive<TIndividual>();
            for (int i = 0; i < initializationIterations; i++)
            {
                TIndividual individual = individualFactory();
                Fitness fitness = fitnessEvaluator(individual);
                Behavior behavior = behaviorEvaluator(individual);
                Entry<TIndividual> entry = new Entry<TIndividual>(individual, fitness, behavior);
                archive.TrySaveInArchive(entry);
            }
            
            for (int i = 0; i < iterations - initializationIterations; i++)
            {
                TIndividual sampledIndividual = archive.SampleRandomSolution();
                TIndividual mutatedIndividual = individualMutator(sampledIndividual);
                Fitness fitness = fitnessEvaluator(mutatedIndividual);
                Behavior behavior = behaviorEvaluator(mutatedIndividual);
                Entry<TIndividual> entry = new Entry<TIndividual>(mutatedIndividual, fitness, behavior);
                archive.TrySaveInArchive(entry);
            }
            
            return archive;
        }
    }
}