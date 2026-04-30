using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLI;
using CLI.Runners;
using Domain.Models;
using MapElites.Args;
using MapElites.Statistics;
using Pokémon.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TilemapAnalysis;
using TilemapAnalysis.Extensions;

Directory.CreateDirectory(FilePaths.OutputPath);

bool shouldCreateStatistics = true;
bool constraintMode = true;

if (args.Length >= 1)
{
    if (args.Contains("--skip-stats") || args.Contains("-s"))
    {
        shouldCreateStatistics = false;
    }

    if (args.Contains("--regular") || args.Contains("-r"))
    {
        constraintMode = false;
    }
}

List<IStatisticsTracker> statisticsTrackers =
    shouldCreateStatistics
        ? constraintMode
            ? [new FitnessTracker(), new CoverageTracker()]
            : [new FitnessTracker(), new CoverageTracker(), new FeasibilityTracker()]
        : [];

KeyCeilings keyCeilings = new(
    flowerPercentageCeiling: 0.2f,
    doorPercentageCeiling: 0.05f,
    variationPercentageCeiling: 1.0f);

int mapDimensions = 20;
int evaluationIterations = 10;
int initializationIterations = 60;
int mutationIterations = 60;

MapElitesArgs mapElitesArgs = new(
    initializationIterations,
    mutationIterations,
    Console.WriteLine,
    FilePaths.OutputPath,
    statisticsTrackers);

using TilemapAnalyzer tilemapAnalyzer = new(FilePaths.TilemapPath);
List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
int tileTypeCount = tilemapAnalyzer.TileTypeCount;
List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

if (constraintMode)
{
    ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs =
        new ConstrainedIndividualHandlerArgs(IndividualHandlerArgs.Create(
                mapDimensions,
                tileTypeCount,
                tileTypes,
                adjacencyRules,
                evaluationIterations,
                keyCeilings),
            0.75f,
            22f);

    ConstrainedMapElitesRunner.Run(mapElitesArgs, constrainedIndividualHandlerArgs);
}
else
{
    IndividualHandlerArgs individualHandlerArgs = IndividualHandlerArgs.Create(
        mapDimensions,
        tileTypeCount,
        tileTypes,
        adjacencyRules,
        evaluationIterations,
        keyCeilings);

    MapElitesRunner.Run(mapElitesArgs, individualHandlerArgs);
}

if (shouldCreateStatistics)
{
    RunPythonStatistics();
}

return;

void RunPythonStatistics()
{
    string pythonScriptsRoot = $"{AppDomain.CurrentDomain.BaseDirectory}/PythonScripts";
    PythonRunner.RunPythonScript($"{pythonScriptsRoot}/statistics_plotter.py", FilePaths.OutputPath);

    Console.WriteLine();
}

void RunTilemapAnalysis()
{
    TilemapAnalyzer tilemapAnalyzer = new(FilePaths.TilemapPath);
    HashSet<string> uniqueHashes = new();

    HashSet<Image<Rgba32>> uniqueImages = tilemapAnalyzer.TileSprites
        .Where(image => uniqueHashes.Add(image.Hash())).ToHashSet();

    (int matches, int notMatches) = uniqueImages.MatchingBorders();
    Console.WriteLine($"Matches: {matches} | NotMatches: {notMatches}");

    int ruleCount = tilemapAnalyzer.GetAdjacencyRules().Count;
    Console.WriteLine($"Adjacency rule count: {ruleCount}");

    int symmetryCount = tilemapAnalyzer.GetSymmetryRules().Count;
}