using Domain.Models;
using Models;
using TilemapAnalysis;
using WFC.Args;
using WFC.Extensions;

namespace WFC.Tests;

public static class LevelFactory
{
    public static Level Create(int dimension, string inputTilemapPath, int maxPropagationDepth)
    {
        List<Vector> positions = new();
        for (int y = 0; y < dimension; y++)
        {
            for (int x = 0; x < dimension; x++)
            {
                positions.Add(new Vector(x, y));
            }
        }

        TilemapAnalyzer analyzer = new TilemapAnalyzer(inputTilemapPath);
        List<TileType> tiles = analyzer.Tiles.Select(tile => tile.Type).ToHashSet().ToList();
        List<AdjacencyRule> rules = analyzer.GetAdjacencyRules();
        
        WfcArgs args = new WfcArgs(positions, tiles, rules, maxPropagationDepth);
        return args.ToLevel();
    }
}