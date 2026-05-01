using System;
using System.Linq;
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
            return RunMapElitesTrial(mapElitesArgs, newIndividualHandlerArgs);
        });

        int populationMin = 10;
        int populationMax = 20;
        int numberOfGenerations = 30;

        var population = new Population(populationMin, populationMax, chromosome); 
        var geneticAlgorithm = new GeneticAlgorithm(
                population, 
                fitness, 
                new TournamentSelection(), // Less aggressive than Elite, maintains diversity
                new UniformCrossover(0.5f), 
                new UniformMutation())
            {
                Termination = new GenerationNumberTermination(numberOfGenerations),
                TaskExecutor = new ParallelTaskExecutor() 
            };
        
        geneticAlgorithm.GenerationRan += (sender, e) =>
        {
            var bestChromosome = geneticAlgorithm.BestChromosome as FloatingPointChromosome;
            double bestSigma = bestChromosome.ToFloatingPoints()[0];
            double fitness = bestChromosome.Fitness.GetValueOrDefault();

            Console.WriteLine($"--- Generation {geneticAlgorithm.GenerationsNumber} ---");
            Console.WriteLine($"Current Best Sigma: {bestSigma:F6}");
            Console.WriteLine($"Reliability from MAP-Elites: {fitness:F2}");
        };
        
        Console.WriteLine("-- Running hyper parameter tuner -- ");
        Console.WriteLine($"Population Minimum: {populationMin}");
        Console.WriteLine($"Population Maximum: {populationMax}");
        Console.WriteLine($"Number of Generations: {numberOfGenerations}");
        
        geneticAlgorithm.Start();
        
        double bestSigma = (geneticAlgorithm.BestChromosome as FloatingPointChromosome).ToFloatingPoints()[0];
        return bestSigma;
    }

    private static double RunMapElitesTrial(
        MapElitesArgs mapElitesArgs, 
        ConstrainedIndividualHandlerArgs individualHandlerArgs)
    {
        ConstrainedIndividualHandler individualHandler =
            new ConstrainedIndividualHandler(individualHandlerArgs);

        return Enumerable
            .Range(0, 3)
            .AsParallel()
            .Select(_ => MapElites.MapElites
                .RunConstrained<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(individualHandler,
                    mapElitesArgs))
            .Average(a => a.GetReliability());
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