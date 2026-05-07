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
        double minSigma = 0.0;
        double maxSigma = 1.0;

        uint steps = 100;
        double stepSize = (maxSigma - minSigma) / steps;

        return Enumerable
            .Sequence(minSigma, maxSigma, stepSize)
            .AsParallel()
            .Select(RunExperimentWithSigma)
            .MaxBy(t => t.fitness).sigma;
        
        (double sigma, double fitness) RunExperimentWithSigma(double sigma)
        {
            ConstrainedIndividualHandlerArgs newArgs = GetNewArgsWithSigma(constrainedIndividualHandlerArgs, sigma);
            double fitness = RunMapElitesTrial(mapElitesArgs, newArgs);
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