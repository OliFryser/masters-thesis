using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Pokémon;
using TilemapAnalysis;
using UnityEditor;
using UnityEngine;
using WFC.Args;

public static class Extensions
{
    public static WfcArgs GetWfcArgs(this Individual individual, Texture2D inputTilemap)
    {
        const int mapDimensions = 20;
        List<Vector> coordinates = Pokémon.LevelGeneration.GetRectangleCoordinates(mapDimensions, mapDimensions).ToList();
        
        string tilemapPath = AssetDatabase.GetAssetPath(inputTilemap);
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        List<Domain.Models.AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules()
            .Concat(tilemapAnalyzer.GetSymmetryRules()).ToList();
        
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, individual.Weights);

        return args;
    }
}