using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLI;
using CLI.Runners;
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

KeyCeilings keyCeilings = new KeyCeilings(
    flowerPercentageCeiling: 0.2f,
    doorPercentageCeiling: 0.05f,
    variationPercentageCeiling: 1.0f);

if (constraintMode)
{
    ConstrainedMapElitesRunner.Run(shouldCreateStatistics, keyCeilings);
}
else
{
    MapElitesRunner.RunMapElites(shouldCreateStatistics, keyCeilings);
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