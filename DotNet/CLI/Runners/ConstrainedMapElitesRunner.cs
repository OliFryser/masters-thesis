using System;
using System.Diagnostics;
using System.Linq;
using MapElites.Args;
using MapElites.Models;
using Pokémon;
using Pokémon.Args;
using Pokémon.Json;
using Pokémon.Statistics;

namespace CLI.Runners;

public static class ConstrainedMapElitesRunner
{
    public static void Run(MapElitesArgs mapElitesArgs,
        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs)
    {
        ConstrainedIndividualHandler constrainedIndividualHandler = new(constrainedIndividualHandlerArgs);

        Stopwatch stopwatch = Stopwatch.StartNew();

        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive =
            MapElites.MapElites.RunConstrained<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(
                constrainedIndividualHandler,
                mapElitesArgs);

        stopwatch.Stop();

        BehaviorSpaceTracker.SaveToFile(
            archive, 
            constrainedIndividualHandler.NumberOfBucketsPerAxis, 
            constrainedIndividualHandlerArgs.FeasibilityThreshold,
            constrainedIndividualHandlerArgs.IndividualHandlerArgs.KeyCeilings,
            FilePaths.DataPath);

        Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");
        
        JsonSerializer.SaveToFile(
            $"{FilePaths.OutputPath}/Archive.json", 
            archive,
            constrainedIndividualHandlerArgs.IndividualHandlerArgs.MapDimensions, 
            (int)constrainedIndividualHandlerArgs.IndividualHandlerArgs.ProblemDomain);
        
        Console.WriteLine("Saved archive to JSON");

        LabLogSaver.SaveLog(
            $"{FilePaths.OutputPath}/Lab.log",
            mapElitesArgs,
            constrainedIndividualHandlerArgs,
            FilePaths.TilemapName);
    }
}