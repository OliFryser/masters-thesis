using System.Collections.Generic;
using System.IO;
using System.Linq;
using MapElites.Args;
using MapElites.Models;
using Newtonsoft.Json;
using Domain;
using Domain.Args;

namespace CLI.Runners;

public static class EvaluationIterationsComparer
{
    private struct EvaluationComparerRunData
    {
        public int EvaluationIterations { get; set; }
        public List<EvaluationComparerEntry> Entries { get; set; }
    }
    
    private struct EvaluationComparerEntry
    {
        public Key Key { get; set; }
        public float Fitness { get; set; }
        public float Feasibility { get; set; }
        public float DeltaFitness { get; set; }
        public float DeltaFeasibility { get; set; }
    }
    
    public static void Run(
        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs,
        MapElitesArgs mapElitesArgs,
        int[] evaluationIterationsToCompare,
        string jsonOutputPath)
    {
        ConstrainedIndividualHandler handler = new ConstrainedIndividualHandler(constrainedIndividualHandlerArgs);
        var archive = 
            MapElites.MapElites.RunConstrained<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(
                handler,
                mapElitesArgs);

        var evaluationEntries = evaluationIterationsToCompare
            .Select(i =>
                new ConstrainedIndividualHandler(constrainedIndividualHandlerArgs
                    .CreateArgsWithEvaluationIterations(i)))
            .Select(h => EvaluateArchiveEntries(archive, h))
            .ToList();
        
        var runData = evaluationIterationsToCompare
            .Zip(evaluationEntries)
            .Select(e => new EvaluationComparerRunData()
            {
                EvaluationIterations = e.First,
                Entries = e.Second,
            })
            .ToList();
        
        File.WriteAllText(
            Path.Combine(jsonOutputPath, "EvaluationIterationData.json"), 
            JsonConvert.SerializeObject(runData, Formatting.Indented));
        
        LabLogSaver.SaveLog(
            $"{FilePaths.OutputPath}/Lab.log",
            mapElitesArgs,
            constrainedIndividualHandlerArgs,
            FilePaths.TilemapName);
    }

    private static List<EvaluationComparerEntry> EvaluateArchiveEntries(
        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive,
        ConstrainedIndividualHandler handler)
    {
        var result = new List<EvaluationComparerEntry>();
        
        var keys = archive.GetKeys();
        foreach (var key in keys)
        {
            if (!archive.TryGet(key, out var entry))
                continue;
            
            if (handler.TryEvaluate(entry.Individual, out var newEntry))
            {
                result.Add(new EvaluationComparerEntry
                {
                    Key = key,
                    Fitness = newEntry.Fitness,
                    DeltaFitness = newEntry.Fitness - entry.Fitness,
                    Feasibility = newEntry.Feasibility,
                    DeltaFeasibility = newEntry.Feasibility - entry.Feasibility,
                });
            }
        }
        
        return result;
    }
    
    private static ConstrainedIndividualHandlerArgs CreateArgsWithEvaluationIterations(
        this ConstrainedIndividualHandlerArgs args,
        int evaluationsIterations)
    {
        var oldIndividualHandlerArgs = args.IndividualHandlerArgs;
        var newIndividualHandlerArgs = 
            IndividualHandlerArgs.Create(
                oldIndividualHandlerArgs.MapDimensions,
                oldIndividualHandlerArgs.TileTypeCount,
                oldIndividualHandlerArgs.TileTypes,
                oldIndividualHandlerArgs.AdjacencyRules,
                evaluationsIterations,
                oldIndividualHandlerArgs.KeyCeilings,
                oldIndividualHandlerArgs.NumberOfBucketsPerAxis,
                oldIndividualHandlerArgs.StandardDeviation,
                oldIndividualHandlerArgs.VariationBehavior,
                oldIndividualHandlerArgs.ProblemDomain,
                oldIndividualHandlerArgs.MutationStrategy);
        return new ConstrainedIndividualHandlerArgs(
            newIndividualHandlerArgs, 
            args.FeasibilityThreshold,
            args.SmoothingFactor);
    }
}