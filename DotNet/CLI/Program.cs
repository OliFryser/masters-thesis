using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLI;
using CLI.Runners;
using Domain.Models;
using MapElites.Args;
using MapElites.Statistics;
using Pokémon;
using Pokémon.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TilemapAnalysis;
using TilemapAnalysis.Extensions;

Directory.CreateDirectory(FilePaths.OutputPath);
Directory.CreateDirectory(FilePaths.DataPath);

bool shouldCreateStatistics = true;

RunMode runMode = RunMode.ConstrainedMapElites;
VariationBehavior variationBehavior = VariationBehavior.UniqueCount;

if (args.Length >= 1)
{
    // Parse statistics flags
    if (args.Contains("--skip-stats") || args.Contains("-s"))
    {
        shouldCreateStatistics = false;
    }
    
    // Parse run flags
    if (args.Contains("--regular") || args.Contains("-r"))
    {
        runMode = RunMode.MapElites;
    }
    else if (args.Contains("--hyper") || args.Contains("-h"))
    {
        runMode = RunMode.HyperParameterTuning;
    }
    else if (args.Contains("--cma") || args.Contains("-c"))
    {
        runMode = RunMode.CmaCme;
    }
    else if (args.Contains("--tilemap") || args.Contains("-t"))
    {
        runMode = RunMode.TileMapAnalysis;
    }
    
    // Parse behavior flags
    if (args.Contains("--entropy") || args.Contains("-e"))
    {
        variationBehavior = VariationBehavior.Entropy;
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
    variationPercentageCeiling: 1.0f);

int mapDimensions = 20;
int evaluationIterations = 50;
int initializationIterations = 500;
int mutationIterations = 10000;
int convergeThreshold = 1000;
int numberOfBucketsPerAxis = 10;
double standardDeviation = 0.1411; // From hyper parameter tuning: 30 generations, minPop 10, maxPop 20

float feasibilityThreshold = 0.60f;
float smoothingFactor = 5f;

int optimizationEmitters = 2;
int feasibilityEmitters = 5;
int randomDirectionEmitters = 3;
double startingStepSize = 0.1411;
int stagnationThreshold = 30;

MapElitesArgs mapElitesArgs = new(
    initializationIterations,
    mutationIterations,
    Console.WriteLine,
    FilePaths.DataPath,
    statisticsTrackers,
    convergeThreshold);

using TilemapAnalyzer tilemapAnalyzer = new(FilePaths.TilemapPath);
List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
int tileTypeCount = tilemapAnalyzer.TileTypeCount;
List<AdjacencyRule> adjacencyRules = tilemapAnalyzer
    .GetAdjacencyRules().Concat(tilemapAnalyzer.GetSymmetryRules()).ToHashSet().ToList();

IndividualHandlerArgs individualHandlerArgs = IndividualHandlerArgs.Create(
    mapDimensions,
    tileTypeCount,
    tileTypes,
    adjacencyRules,
    evaluationIterations,
    keyCeilings,
    numberOfBucketsPerAxis,
    standardDeviation, 
    variationBehavior);

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
        CmaCmeRunner.Run(mapElitesArgs, constrainedIndividualHandlerArgs, emitterConfiguration, startingStepSize, stagnationThreshold);
        break;
    case RunMode.TileMapAnalysis:
        RunTilemapAnalysis();
        break;
    case RunMode.HyperParameterTuning:
        mapElitesArgs = new MapElitesArgs(
            mapElitesArgs.InitializationIterations,
            mapElitesArgs.MutationIterations,
            _ => {},
            mapElitesArgs.StatisticsOutputPath,
            [],
            mapElitesArgs.ConvergeThreshold);
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
    LabLogSaver.SaveLog(Path.Combine(FilePaths.OutputPath, "Lab.log"), mapElitesArgs, constrainedIndividualHandlerArgs, FilePaths.TilemapName);
}