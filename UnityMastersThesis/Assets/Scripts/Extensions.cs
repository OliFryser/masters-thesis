using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Domain;
using TilemapAnalysis;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using WFC.Args;

public static class Extensions
{
    #if UNITY_EDITOR
    public static WfcArgs GetWfcArgs(this Individual individual, Texture2D inputTilemap)
    {
        const int mapDimensions = 20;
        List<Vector> coordinates = LevelGeneration.GetRectangleCoordinates(mapDimensions, mapDimensions).ToList();
        
        string tilemapPath = AssetDatabase.GetAssetPath(inputTilemap);
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(tilemapPath);
        List<TileType> tileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        List<Core.Models.AdjacencyRule> adjacencyRules = tilemapAnalyzer.GetAdjacencyRules()
            .Concat(tilemapAnalyzer.GetSymmetryRules()).ToList();
        
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, individual.Weights);

        return args;
    }
    #endif
}