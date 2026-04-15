using System;
using System.Diagnostics;
using MapElites.Args;
using MapElites.Models;
using Pokémon;

string baseDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../..";
string resourceDirectory = $"{baseDirectory}/Resources";
string tilemapPath = $"{resourceDirectory}/Tilemaps/PalletTown.png";
// Save in folder named timestamp
string outputPath = $"{baseDirectory}/Output/MapElites/{DateTime.Now:yyyyMMdd-HHmmssffff}";

MapElitesArgs mapElitesArgs = new(100, 1000, Console.WriteLine, outputPath);
IndividualHandler individualHandler = new(tilemapPath);

Stopwatch stopwatch = Stopwatch.StartNew();
Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(individualHandler, mapElitesArgs);
stopwatch.Stop();

Console.WriteLine($"Finished MAP-Elites in:  {stopwatch.Elapsed.TotalSeconds} ms");

Individual maxFitnessIndividual = archive.GetMaxFitnessIndividual();

Console.Write("Best Individual Weights: ");
foreach (var keyValuePair in maxFitnessIndividual.Weights)
{
    Console.Write($"{keyValuePair.Key.Id}: {keyValuePair.Value}, ");
}

Console.WriteLine();