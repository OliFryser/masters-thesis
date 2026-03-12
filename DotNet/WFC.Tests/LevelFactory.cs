using Domain.Models;
using Models;
using TilemapAnalysis;
using WFC.Args;
using WFC.Extensions;

namespace WFC.Tests;

public class LevelFactory
{
    public enum Size
    {
        Small,
        Medium,
        Large
    }

    public Level Create(Size size, string inputTilemapPath, int maxPropagationDepth)
    {
        List<Vector> positions = new();
        int dimension = Dimension(size);
        for (int y = 0; y < dimension; y++)
        {
            for (int x = 0; x < dimension; x++)
            {
                positions.Add(new Vector(x, y));
            }
        }

        TilemapAnalyzer analyzer = new TilemapAnalyzer(inputTilemapPath);
        List<TileType> tiles = analyzer.Tiles.Select(tile => tile.Type).ToList();
        List<AdjacencyRule> rules = analyzer.GetAdjacencyRules();
        
        WfcArgs args = new WfcArgs(positions, tiles, rules, maxPropagationDepth);
        return args.ToLevel();
    }

    private int Dimension(Size size) => size switch
    {
        Size.Small => 10,
        Size.Medium => 25,
        Size.Large => 50,
        _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
    };
}