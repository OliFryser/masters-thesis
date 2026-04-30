using System;
using System.Diagnostics;
using MapElites.Args;
using MapElites.Models;
using Pokémon;
using Pokémon.Args;
using Pokémon.Json;

namespace CLI.Runners;

public static class MapElitesRunner
{
    public static void Run(MapElitesArgs mapElitesArgs, IndividualHandlerArgs individualHandlerArgs)
    {
        IndividualHandler individualHandler = new(individualHandlerArgs);

        Stopwatch stopwatch = Stopwatch.StartNew();
        Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(individualHandler, mapElitesArgs);
        stopwatch.Stop();

        Console.WriteLine($"Finished MAP-Elites in: {stopwatch.Elapsed.TotalSeconds} ms");

        JsonSerializer.SaveToFile($"{FilePaths.OutputPath}/Archive.json", archive, individualHandlerArgs.MapDimensions);

        Console.WriteLine("Saved archive to JSON.");

        LabLogSaver.SaveLog($"{FilePaths.OutputPath}/Lab.log", mapElitesArgs, individualHandlerArgs,
            FilePaths.TilemapName);

        // var saveData = JsonSerializer.ReadFromFile($"{FilePaths.OutputPath}/Archive.json");
    }
}