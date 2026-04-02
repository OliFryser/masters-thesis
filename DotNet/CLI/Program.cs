using System;
using System.Diagnostics;
using MapElites.Args;
using MapElites.Models;
using Pokémon;

string resourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../Resources/";
string tilemapPath = $"{resourceDirectory}Tilemaps/PalletTown.png";

MapElitesArgs mapElitesArgs = new MapElitesArgs(100, 1000, Console.WriteLine);
IndividualHandler individualHandler = new IndividualHandler(tilemapPath);

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