using System;
using MapElites;
using MapElites.Args;
using MapElites.Models;
using Pokémon;

string resourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../Resources/";
string tilemapPath = $"{resourceDirectory}Tilemaps/PalletTown.png";

MapElitesArgs mapElitesArgs = new MapElitesArgs(5, 5, Console.WriteLine);
IndividualHandler populationManager = new(tilemapPath);
Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(populationManager, mapElitesArgs);
