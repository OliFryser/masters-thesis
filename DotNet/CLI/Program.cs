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
Directory.CreateDirectory(FilePaths.DataPath);

bool shouldCreateStatistics = true;

// No args (default) = CmaCme
RunMode runMode = RunMode.CmaCme;

if (args.Length >= 1)
{
    if (args.Contains("--skip-stats") || args.Contains("-s"))
    {
        shouldCreateStatistics = false;
    }

    if (args.Contains("--regular") || args.Contains("-r"))
    {
        runMode = RunMode.MapElites;
    }
    else if (args.Contains("--hyper") || args.Contains("-h"))
    {
        runMode = RunMode.HyperParameterTuning;
    }
    else if (args.Contains("--constrained") || args.Contains("-c"))
    {
        runMode = RunMode.ConstrainedMapElites;
    }
    else if (args.Contains("--tilemap") || args.Contains("-t"))
    {
        runMode = RunMode.TileMapAnalysis;
    }
}

List<IStatisticsTracker> statisticsTrackers;
if (shouldCreateStatistics)
{
    statisticsTrackers = [new FitnessTracker(), new CoverageTracker(), new ReliabilityTracker()];
    if (runMode is RunMode.ConstrainedMapElites or RunMode.CmaCme)
    {
        statisticsTrackers.Add(new FeasibilityTracker());
    }
}
else
{
    statisticsTrackers = [];
}

KeyCeilings keyCeilings = new(
    flowerPercentageCeiling: 0.2f,
    doorPercentageCeiling: 0.05f,
    variationPercentageCeiling: 1.0f);

int mapDimensions = 20;
int evaluationIterations = 20;
int initializationIterations = 100;
int mutationIterations = 1000;
int numberOfBucketsPerAxis = 10;
double standardDeviation = 0.1411; // From hyper parameter tuning: 30 generations, minPop 10, maxPop 20

float feasibilityThreshold = 0.75f;
float smoothingFactor = 5f;

int optimizationEmitters = 1;
int feasibilityEmitters = 1;
int randomDirectionEmitters = 1;
double startingStepSize = 0.3;

MapElitesArgs mapElitesArgs = new(
    initializationIterations,
    mutationIterations,
    Console.WriteLine,
    FilePaths.DataPath,
    statisticsTrackers);

using TilemapAnalyzer tilemapAnalyzer = new(FilePaths.TilemapPath);
List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
int tileTypeCount = tilemapAnalyzer.TileTypeCount;
List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

IndividualHandlerArgs individualHandlerArgs = IndividualHandlerArgs.Create(
    mapDimensions,
    tileTypeCount,
    tileTypes,
    adjacencyRules,
    evaluationIterations,
    keyCeilings,
    numberOfBucketsPerAxis,
    standardDeviation);

ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs = 
    new(individualHandlerArgs, feasibilityThreshold, smoothingFactor);

switch (runMode)
{
    case RunMode.MapElites:
        MapElitesRunner.Run(mapElitesArgs, individualHandlerArgs);
        break;
    case RunMode.ConstrainedMapElites:
        ConstrainedMapElitesRunner.Run(mapElitesArgs, constrainedIndividualHandlerArgs);
        break;
    case RunMode.CmaCme:
        var emitterConfiguration =
            new EmitterConfiguration(optimizationEmitters, feasibilityEmitters, randomDirectionEmitters);
        CmaCmeRunner.Run(mapElitesArgs, constrainedIndividualHandlerArgs, emitterConfiguration, startingStepSize);
        break;
    case RunMode.TileMapAnalysis:
        RunTilemapAnalysis();
        break;
    case RunMode.HyperParameterTuning:
        mapElitesArgs = new MapElitesArgs(
            mapElitesArgs.InitializationIterations,
            mapElitesArgs.InitializationIterations,
            _ => {},
            mapElitesArgs.StatisticsOutputPath,
            []);
        RunHyperParameterTuning();
        return;
    default:
        throw new ArgumentOutOfRangeException();
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

void RunHyperParameterTuning()
{
    var tunedSigma = HyperParameterTuner.FindBestSigma(mapElitesArgs, constrainedIndividualHandlerArgs);
    Console.WriteLine($"Best sigma found: {tunedSigma}");
}