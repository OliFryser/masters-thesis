using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using MapElites.Args;
using MapElites.Models;
using MapElites.Statistics;
using Pokémon;
using Pokémon.Args;

namespace CLI.Runners;

public static class HyperParameterTuner
{
    public static double FindBestSigma(
        MapElitesArgs mapElitesArgs,
        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs)
    {
        double minSigma = 0.0;
        double maxSigma = 1.0;

        uint steps = 20;
        double stepSize = (maxSigma - minSigma) / steps;

        List<(double, double)> loggerEntries = new List<(double, double)>();
        
        double bestSigma = Enumerable
            .Sequence(minSigma, maxSigma, stepSize)
            .AsParallel()
            .Select(RunExperimentWithSigma)
            .MaxBy(t => t.fitness).sigma;
        
        
        using StreamWriter streamWriter = new StreamWriter(Path.Combine(FilePaths.OutputPath, "HyperParam.Log"));
        
        loggerEntries.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        loggerEntries.ForEach(e => streamWriter.WriteLine($"Sigma: {e.Item1:F3}, Fitness: {e.Item2:F3}"));
        return bestSigma;
        
        (double sigma, double fitness) RunExperimentWithSigma(double sigma, int iteration)
        {
            Console.WriteLine($"Running iteration {iteration} out of {steps} ({iteration / (double)steps * 100:F0} %) with sigma: {sigma.ToString("F2", CultureInfo.InvariantCulture)}");
            ConstrainedIndividualHandlerArgs newArgs = GetNewArgsWithSigma(constrainedIndividualHandlerArgs, sigma);
            double fitness = RunMapElitesTrial(mapElitesArgs, newArgs);
            lock (loggerEntries)
            {
                loggerEntries.Add((sigma, fitness));
            }
            return (sigma, fitness);
        }
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
            sigma,
            oldIndividualHandlerArgs.VariationBehavior);
        return new ConstrainedIndividualHandlerArgs(individualHandlerArgs, oldArgs.FeasibilityThreshold,
            oldArgs.SmoothingFactor);
    }
}