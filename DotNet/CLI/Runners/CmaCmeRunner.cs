using System;
using System.Diagnostics;
using MapElites.Args;
using MapElites.Models;
using Pokémon;
using Pokémon.Args;
using Pokémon.Json;
using Pokémon.Statistics;

namespace CLI.Runners;

public static class CmaCmeRunner
{
    public static void Run(
        MapElitesArgs mapElitesArgs, 
        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs,
        EmitterConfiguration emitterConfiguration,
        double startingStepSize)
    {
        ConstrainedIndividualHandler constrainedIndividualHandler = new(constrainedIndividualHandlerArgs);

        Stopwatch stopwatch = Stopwatch.StartNew();

        CmaCmeArgs cmaCmeArgs = 
            new(mapElitesArgs, constrainedIndividualHandler, emitterConfiguration, startingStepSize);
        
        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive =
            CmaCme.Run(cmaCmeArgs);

        stopwatch.Stop();

        BehaviorSpaceTracker.SaveToFile(archive, constrainedIndividualHandler.NumberOfBucketsPerAxis, FilePaths.DataPath);

        Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");

        JsonSerializer.SaveToFile($"{FilePaths.OutputPath}/Archive.json", archive,
            constrainedIndividualHandlerArgs.IndividualHandlerArgs.MapDimensions);

        Console.WriteLine("Saved archive to JSON");

        LabLogSaver.SaveLog(
            $"{FilePaths.OutputPath}/Lab.log",
            cmaCmeArgs,
            constrainedIndividualHandlerArgs,
            FilePaths.TilemapName);
    }
}