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
ProblemDomain problemDomain = ProblemDomain.Pokemon;
MutationStrategy mutationStrategy = MutationStrategy.AllTiles;

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

    string? domainFlag = args.FirstOrDefault(s => s.StartsWith("-d") || s.StartsWith("--domain"));
    if (domainFlag != null)
    {
        int domainNumber = int.Parse(domainFlag.Split("=")[1]);
        problemDomain = (ProblemDomain)domainNumber;
    }
    
    string? mutationFlag = args.FirstOrDefault(s => s.StartsWith("-m") || s.StartsWith("--mutation"));
    if (mutationFlag != null)
    {
        int mutationNumber = int.Parse(mutationFlag.Split("=")[1]);
        mutationStrategy = (MutationStrategy)mutationNumber;
        if (mutationStrategy == MutationStrategy.CmaCme)
        {
            runMode = RunMode.CmaCme;
        }
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

KeyCeilings keyCeilings = problemDomain switch
{
    ProblemDomain.Letters => new(
        specialTileCeiling: 1.0f,
        variationPercentageCeiling: 1.0f),
    ProblemDomain.Arrows => new(
        specialTileCeiling: 1.0f,
        variationPercentageCeiling: 1.0f),
    ProblemDomain.Pokemon => new(
        specialTileCeiling: 0.2f,
        variationPercentageCeiling: 1.0f),
    _ => throw new ArgumentOutOfRangeException()
};

int mapDimensions = 10;
int evaluationIterations = 50;
int initializationIterations = 250;
int mutationIterations = 5000;
int convergeThreshold = 500;
int numberOfBucketsPerAxis = 10;

double standardDeviation = problemDomain switch
{
    ProblemDomain.Letters => 0.25,
    ProblemDomain.Arrows => 0.3,
    ProblemDomain.Pokemon => 0.2,
    _ => throw new ArgumentOutOfRangeException()
};

float feasibilityThreshold = 0.75f;
float smoothingFactor = 5f;

int optimizationEmitters = 2;
int feasibilityEmitters = 5;
int randomDirectionEmitters = 3;
double startingStepSize = 0.1411;
int stagnationThreshold = 30;

int mapId = problemDomain switch
{
    ProblemDomain.Letters => 0,
    ProblemDomain.Arrows => 1,
    ProblemDomain.Pokemon => 2,
    _ => throw new ArgumentOutOfRangeException()
};

MapElitesArgs mapElitesArgs = new(
    initializationIterations,
    mutationIterations,
    Console.WriteLine,
    FilePaths.DataPath,
    statisticsTrackers,
    convergeThreshold);

FilePaths.TilemapName = problemDomain switch
{
    ProblemDomain.Letters => "Letters.png",
    ProblemDomain.Arrows => "Arrows.png",
    ProblemDomain.Pokemon => "PalletTown.png",
    _ => throw new ArgumentOutOfRangeException()
};

using TilemapAnalyzer tilemapAnalyzer = new(FilePaths.TilemapPath);
List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
int tileTypeCount = tilemapAnalyzer.TileTypeCount;
List<AdjacencyRule> adjacencyRules = new List<AdjacencyRule>();

adjacencyRules.AddRange(
    problemDomain switch
    {
        ProblemDomain.Letters or ProblemDomain.Arrows => tilemapAnalyzer.GetSymmetryRules(),
        ProblemDomain.Pokemon =>
            tilemapAnalyzer.GetAdjacencyRules().Concat(tilemapAnalyzer.GetSymmetryRules()).ToHashSet().ToList(),
        _ => throw new ArgumentOutOfRangeException()
    }
);
   
IndividualHandlerArgs individualHandlerArgs = IndividualHandlerArgs.Create(
    mapDimensions,
    tileTypeCount,
    tileTypes,
    adjacencyRules,
    evaluationIterations,
    keyCeilings,
    numberOfBucketsPerAxis,
    standardDeviation, 
    variationBehavior,
    problemDomain,
    mutationStrategy);

ConstrainedIndividualHandlerArgs constrainedIndividualHandlerArgs = 
    new(individualHandlerArgs, feasibilityThreshold, smoothingFactor);

switch (runMode)
{
    case RunMode.MapElites:
        MapElitesRunner.Run(mapElitesArgs, individualHandlerArgs);
        break;
    case RunMode.ConstrainedMapElites:
        ConstrainedMapElitesRunner.Run(mapElitesArgs, constrainedIndividualHandlerArgs, mapId);
        break;
    case RunMode.CmaCme:
        var emitterConfiguration =
            new EmitterConfiguration(optimizationEmitters, feasibilityEmitters, randomDirectionEmitters);
        CmaCmeRunner.Run(mapElitesArgs, constrainedIndividualHandlerArgs, emitterConfiguration, startingStepSize, stagnationThreshold, mapId);
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
    using TilemapAnalyzer tilemapAnalyzer = new(FilePaths.TilemapPath);
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