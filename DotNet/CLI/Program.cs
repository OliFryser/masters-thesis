using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CLI;
using Domain.Models;
using MapElites.Args;
using MapElites.Models;
using Pokémon;
using Pokémon.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TilemapAnalysis;
using TilemapAnalysis.Extensions;
using JsonSerializer = Pokémon.Json.JsonSerializer;

string baseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../..";
string resourceDirectory = $"{baseDirectory}/Resources";
string tilemapPath = $"{resourceDirectory}/Tilemaps/PalletTown.png";

// Save in folder named timestamp
string outputPath = $"{baseDirectory}/Output/MapElites/{DateTime.Now:yyyyMMdd-HHmmss}";
// Ensure path exists
Directory.CreateDirectory(outputPath);

RunMapElites();
RunPythonStatistics();

return;

void RunMapElites()
{
    using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
    List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
    int tileTypeCount = tilemapAnalyzer.TileTypeCount;
    List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

    int mapDimension = 20;
    int evaluationIterations = 5;
    
    IndividualHandlerArgs individualHandlerArgs =
        IndividualHandlerArgs.Create(
            mapDimension, 
            tileTypeCount, 
            tileTypes, 
            adjacencyRules, 
            evaluationIterations);

    MapElitesArgs mapElitesArgs = new(10, 10, Console.WriteLine, outputPath);
    IndividualHandler individualHandler = new(individualHandlerArgs);

    Stopwatch stopwatch = Stopwatch.StartNew();
    Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(individualHandler, mapElitesArgs);
    stopwatch.Stop();

    Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");
    
    JsonSerializer.SaveToFile($"{outputPath}/Archive.json", archive, mapDimension);
    
    Console.WriteLine("Saved archive to JSON");
}

void RunPythonStatistics()
{
    string pythonScriptsRoot = $"{AppDomain.CurrentDomain.BaseDirectory}/PythonScripts";
    PythonRunner.RunPythonScript($"{pythonScriptsRoot}/statistics_plotter.py", outputPath);

    Console.WriteLine();
}

void RunTilemapAnalysis()
{
    TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
    HashSet<string> uniqueHashes = new HashSet<string>();

    HashSet<Image<Rgba32>> uniqueImages = tilemapAnalyzer.TileSprites
        .Where(image => uniqueHashes.Add(image.Hash())).ToHashSet();

    (int matches, int notMatches) = uniqueImages.MatchingBorders();
    Console.WriteLine($"Matches: {matches} | NotMatches: {notMatches}");

    int ruleCount = tilemapAnalyzer.GetAdjacencyRules().Count;
    Console.WriteLine($"Adjacency rule count: {ruleCount}");

    int symmetryCount = tilemapAnalyzer.GetSymmetryRules().Count;
}