using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CLI;
using Domain.Models;
using MapElites.Args;
using MapElites.Models;
using MapElites.Statistics;
using Pokémon;
using Pokémon.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TilemapAnalysis;
using TilemapAnalysis.Extensions;
using JsonSerializer = Pokémon.Json.JsonSerializer;

Directory.CreateDirectory(FilePaths.OutputPath);

ConstrainedMapElitesRunner.Run();

RunMapElites();
RunPythonStatistics();

return;

void RunMapElites()
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

    Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");
    
    JsonSerializer.SaveToFile($"{FilePaths.OutputPath}/Archive.json", archive, mapDimension);
    
    Console.WriteLine("Saved archive to JSON");
    
    LabLogSaver.SaveLog($"{FilePaths.OutputPath}/Lab.log", mapElitesArgs, individualHandlerArgs, FilePaths.TilemapName);
}

void RunPythonStatistics()
{
    string pythonScriptsRoot = $"{AppDomain.CurrentDomain.BaseDirectory}/PythonScripts";
    PythonRunner.RunPythonScript($"{pythonScriptsRoot}/statistics_plotter.py", FilePaths.OutputPath);

    Console.WriteLine();
}

void RunTilemapAnalysis()
{
    TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(FilePaths.TilemapPath);
    HashSet<string> uniqueHashes = new HashSet<string>();

    HashSet<Image<Rgba32>> uniqueImages = tilemapAnalyzer.TileSprites
        .Where(image => uniqueHashes.Add(image.Hash())).ToHashSet();

    (int matches, int notMatches) = uniqueImages.MatchingBorders();
    Console.WriteLine($"Matches: {matches} | NotMatches: {notMatches}");

    int ruleCount = tilemapAnalyzer.GetAdjacencyRules().Count;
    Console.WriteLine($"Adjacency rule count: {ruleCount}");

    int symmetryCount = tilemapAnalyzer.GetSymmetryRules().Count;
}