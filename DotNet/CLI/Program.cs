using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLI;
using CLI.Runners;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TilemapAnalysis;
using TilemapAnalysis.Extensions;

Directory.CreateDirectory(FilePaths.OutputPath);

bool shouldCreateStatistics = true;
bool constraintMode = false;

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

if (constraintMode)
{
    ConstrainedMapElitesRunner.Run(shouldCreateStatistics);
}
else
{
    MapElitesRunner.RunMapElites(shouldCreateStatistics);
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