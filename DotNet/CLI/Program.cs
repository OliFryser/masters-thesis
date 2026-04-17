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
using Pokémon.Json;
using TilemapAnalysis;

string baseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../..";
string pythonScriptsRoot = $"{AppDomain.CurrentDomain.BaseDirectory}/PythonScripts";
string resourceDirectory = $"{baseDirectory}/Resources";
string tilemapPath = $"{resourceDirectory}/Tilemaps/PalletTown.png";

// Save in folder named timestamp
string outputPath = $"{baseDirectory}/Output/MapElites/{DateTime.Now:yyyyMMdd-HHmmss}";
// Ensure path exists
Directory.CreateDirectory(outputPath);

using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
int tileTypeCount = tilemapAnalyzer.TileTypeCount;
List<AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

int mapDimension = 20;

IndividualHandlerArgs individualHandlerArgs =
    IndividualHandlerArgs.Create(mapDimension, tileTypeCount, tileTypes, adjacencyRules);

MapElitesArgs mapElitesArgs = new(10, 10, Console.WriteLine, outputPath);
IndividualHandler individualHandler = new(individualHandlerArgs);

Stopwatch stopwatch = Stopwatch.StartNew();
Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(individualHandler, mapElitesArgs);
stopwatch.Stop();

Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");

string json = JsonSerializer.ConvertToJson(mapDimension, archive);
JsonSerializer.WriteToFile($"{outputPath}/Archive.json", json);

PythonRunner.RunPythonScript($"{pythonScriptsRoot}/statistics_plotter.py", outputPath);

Console.WriteLine();