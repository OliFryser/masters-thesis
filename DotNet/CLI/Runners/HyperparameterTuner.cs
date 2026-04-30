using System;
using GeneticSharp;
using MapElites.Args;
using MapElites.Models;
using Pokémon;
using Pokémon.Args;

namespace CLI.Runners;

public static class HyperParameterTuner
{
    public static double FindBestSigma(
        MapElitesArgs mapElitesArgs, 
        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs)
    {
        // Range: 0.0001 to 1.0, using 16 bits of precision
        FloatingPointChromosome chromosome = new FloatingPointChromosome(
            [0.0001],
            [1.0],
            [16],
            [4]);

        FuncFitness fitness = new FuncFitness(c =>
        {
            FloatingPointChromosome? floatingPointChromosome = c as FloatingPointChromosome;
            double sigma = floatingPointChromosome.ToFloatingPoints()[0];

            ConstrainedIndividualHandlerArgs newIndividualHandlerArgs = GetNewArgsWithSigma(constrainedIndividualHandlerArgs, sigma);
            return RunMapElitesTrial(sigma, mapElitesArgs, newIndividualHandlerArgs);
        });

        Population population = new Population(10, 20, chromosome);
        GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(
            population, 
            fitness, 
            new EliteSelection(), 
            new UniformCrossover(), 
            new UniformMutation())
        {
            Termination = new GenerationNumberTermination(50),
        };
        
        geneticAlgorithm.GenerationRan += (sender, e) =>
        {
            var bestChromosome = geneticAlgorithm.BestChromosome as FloatingPointChromosome;
            double bestSigma = bestChromosome.ToFloatingPoints()[0];
            double fitness = bestChromosome.Fitness.GetValueOrDefault();

            Console.WriteLine($"--- Generation {geneticAlgorithm.GenerationsNumber} ---");
            Console.WriteLine($"Current Best Sigma: {bestSigma:F6}");
            Console.WriteLine($"QD-Score: {fitness:F2}");
        };
        
        geneticAlgorithm.Start();

        double bestSigma = (geneticAlgorithm.BestChromosome as FloatingPointChromosome).ToFloatingPoints()[0];
        return bestSigma;
    }

    private static double RunMapElitesTrial(
        double sigma, 
        MapElitesArgs mapElitesArgs, 
        ConstrainedIndividualHandlerArgs individualHandlerArgs)
    {
        ConstrainedIndividualHandler individualHandler =
            new ConstrainedIndividualHandler(individualHandlerArgs);
        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive = MapElites.MapElites
            .RunConstrained<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(individualHandler,
                mapElitesArgs);

        return archive.GetAverageFitness();
    }

    private static ConstrainedIndividualHandlerArgs GetNewArgsWithSigma(
        ConstrainedIndividualHandlerArgs oldArgs,
        double sigma)
    {
        IndividualHandlerArgs oldIndividualHandlerArgs = oldArgs.IndividualHandlerArgs;
        IndividualHandlerArgs individualHandlerArgs = IndividualHandlerArgs.Create(
            oldIndividualHandlerArgs.MapDimensions,
            oldIndividualHandlerArgs.TileTypeCount,
            oldIndividualHandlerArgs.TileTypes,
            oldIndividualHandlerArgs.AdjacencyRules,
            oldIndividualHandlerArgs.EvaluationIterations,
            oldIndividualHandlerArgs.KeyCeilings,
            oldIndividualHandlerArgs.NumberOfBucketsPerAxis,
            sigma);
        return new ConstrainedIndividualHandlerArgs(individualHandlerArgs, oldArgs.FeasibilityThreshold,
            oldArgs.SmoothingFactor);
    }
}