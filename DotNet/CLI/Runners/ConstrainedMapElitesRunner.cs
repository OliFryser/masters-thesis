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

public static class ConstrainedMapElitesRunner
{
    public static void Run(bool shouldCreateStatistics, KeyCeilings keyCeilings)
    {
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(FilePaths.TilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        int tileTypeCount = tilemapAnalyzer.TileTypeCount;
        List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

        int mapDimension = 20;
        int evaluationIterations = 50;
        int initializationIterations = 20;
        int mutationIterations = 50;

        ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs =
            new ConstrainedIndividualHandlerArgs(IndividualHandlerArgs.Create(
                    mapDimension,
                    tileTypeCount,
                    tileTypes,
                    adjacencyRules,
                    evaluationIterations, 
                    keyCeilings),
                0.75f,
                22f);

        List<IStatisticsTracker> statisticsTrackers =
            shouldCreateStatistics ? [new FitnessTracker(), new CoverageTracker(), new FeasibilityTracker()] : [];
        
        ConstrainedIndividualHandler constrainedIndividualHandler = new(constrainedIndividualHandlerArgs);

        MapElitesArgs mapElitesArgs = new(
            initializationIterations,
            mutationIterations,
            Console.WriteLine,
            FilePaths.OutputPath,
            statisticsTrackers);
        
        Stopwatch stopwatch = Stopwatch.StartNew();

        ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive =
            MapElites.MapElites.RunConstrained<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>(
                constrainedIndividualHandler,
                mapElitesArgs);

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