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

namespace CLI.Runners;

public static class MapElitesRunner
{
    public static void RunMapElites(bool shouldCreateStatistics, KeyCeilings keyCeilings)
    {
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(FilePaths.TilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        int tileTypeCount = tilemapAnalyzer.TileTypeCount;
        List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

        int mapDimension = 20;
        int evaluationIterations = 10;
        int initializationIterations = 60;
        int mutationIterations = 60;
    
        IndividualHandlerArgs individualHandlerArgs =
            IndividualHandlerArgs.Create(
                mapDimension, 
                tileTypeCount, 
                tileTypes, 
                adjacencyRules, 
                evaluationIterations,
                keyCeilings);

        List<IStatisticsTracker> statisticsTrackers = 
            shouldCreateStatistics ? [new FitnessTracker(), new CoverageTracker()] : [];
    
        MapElitesArgs mapElitesArgs = 
            new(initializationIterations, mutationIterations, Console.WriteLine, FilePaths.OutputPath, statisticsTrackers);
        IndividualHandler individualHandler = new(individualHandlerArgs);

        Stopwatch stopwatch = Stopwatch.StartNew();
        Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(individualHandler, mapElitesArgs);
        stopwatch.Stop();

        Console.WriteLine($"Finished MAP-Elites in: {stopwatch.Elapsed.TotalSeconds} ms");
    
        JsonSerializer.SaveToFile($"{FilePaths.OutputPath}/Archive.json", archive, mapDimension);
    
        Console.WriteLine("Saved archive to JSON.");
    
        LabLogSaver.SaveLog($"{FilePaths.OutputPath}/Lab.log", mapElitesArgs, individualHandlerArgs, FilePaths.TilemapName);
        
        // var saveData = JsonSerializer.ReadFromFile($"{FilePaths.OutputPath}/Archive.json");
    }
}