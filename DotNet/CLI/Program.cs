using System;
using MapElites.Args;
using MapElites.Models;
using Pokémon;

string resourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../Resources/";
string tilemapPath = $"{resourceDirectory}Tilemaps/PalletTown.png";

MapElitesArgs mapElitesArgs = new MapElitesArgs(5, 5, Console.WriteLine);
PopulationManager populationManager = new PopulationManager(tilemapPath);
Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(populationManager, mapElitesArgs);
