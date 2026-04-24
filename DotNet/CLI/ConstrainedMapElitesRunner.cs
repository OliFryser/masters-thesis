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

public static class ConstrainedMapElitesRunner
{
    public static void Run()
    {
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(FilePaths.TilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        int tileTypeCount = tilemapAnalyzer.TileTypeCount;
        List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

        int mapDimension = 5;
        int evaluationIterations = 2;
        int initializationIterations = 50;
        int mutationIterations = 50;
    
        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs =
            new ConstrainedIndividualHandlerArgs(IndividualHandlerArgs.Create(
                mapDimension, 
                tileTypeCount, 
                tileTypes, 
                adjacencyRules, 
                evaluationIterations), 
                0.8f);

        List<IStatisticsTracker> statisticsTrackers = [new FitnessTracker(), new CoverageTracker()];
    
        MapElitesArgs mapElitesArgs = new(initializationIterations, mutationIterations, Console.WriteLine, FilePaths.OutputPath, statisticsTrackers);
        ConstrainedIndividualHandler constrainedIndividualHandler = new(constrainedIndividualHandlerArgs);

        Stopwatch stopwatch = Stopwatch.StartNew();
        IArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive = 
            MapElites.MapElites.RunConstrained<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(constrainedIndividualHandler, mapElitesArgs);
        stopwatch.Stop();

        Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");
    
        JsonSerializer.SaveToFile($"{FilePaths.OutputPath}/Archive.json", archive, mapDimension);
    
        Console.WriteLine("Saved archive to JSON");
    
        LabLogSaver.SaveLog(
            $"{FilePaths.OutputPath}/Lab.log", 
            mapElitesArgs, 
            constrainedIndividualHandlerArgs, 
            FilePaths.TilemapName);
        
        // Get the archive like this:
        // var saveData = JsonSerializer.ReadConstrainedSaveDataFromFile($"{FilePaths.OutputPath}/Archive.json");
        // saveData.Archive;
    }
}