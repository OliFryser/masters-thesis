using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.Models;
using MapElites.Args;
using MapElites.Models;
using MapElites.Statistics;
using Pokémon;
using Pokémon.Args;
using Pokémon.Json;
using TilemapAnalysis;

namespace CLI;

public static class MapElitesRunner
{
    public static void RunMapElites()
    {
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(FilePaths.TilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        int tileTypeCount = tilemapAnalyzer.TileTypeCount;
        List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

        int mapDimension = 5;
        int evaluationIterations = 2;
        int initializationIterations = 50;
        int mutationIterations = 50;
    
        IndividualHandlerArgs individualHandlerArgs =
            IndividualHandlerArgs.Create(
                mapDimension, 
                tileTypeCount, 
                tileTypes, 
                adjacencyRules, 
                evaluationIterations);

        List<IStatisticsTracker> statisticsTrackers = [new FitnessTracker(), new CoverageTracker()];
    
        MapElitesArgs mapElitesArgs = 
            new(initializationIterations, mutationIterations, Console.WriteLine, FilePaths.OutputPath, statisticsTrackers);
        IndividualHandler individualHandler = new(individualHandlerArgs);

        Stopwatch stopwatch = Stopwatch.StartNew();
        IArchive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(individualHandler, mapElitesArgs);
        stopwatch.Stop();

        Console.WriteLine($"Finished MAP-Elites in: {stopwatch.Elapsed.TotalSeconds} ms");
    
        JsonSerializer.SaveToFile($"{FilePaths.OutputPath}/Archive.json", archive, mapDimension);
    
        Console.WriteLine("Saved archive to JSON.");
    
        LabLogSaver.SaveLog($"{FilePaths.OutputPath}/Lab.log", mapElitesArgs, individualHandlerArgs, FilePaths.TilemapName);
    }
}