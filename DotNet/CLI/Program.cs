using System;
using MapElites.Args;
using MapElites.Models;
using Pokémon;

var resourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../Resources/";
var tilemapPath = resourceDirectory + "Tilemaps/PalletTown.png";

MapElitesArgs mapElitesArgs = new MapElitesArgs(10, 20);
PopulationManager populationManager = new PopulationManager(tilemapPath);
Archive<Key, Entry, Individual, Behavior> archive = MapElites.MapElites.Run(populationManager, mapElitesArgs);
