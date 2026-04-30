using System.Collections;
using Domain.Models;
using TilemapAnalysis;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;

namespace WFC.Tests.Helpers;

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
        
        WfcArgs args = new WfcArgs(positions, tiles, rules, analyzer.Weights, analyzer.TileCount);
        return args.ToLevel();
    }
    
    /// <summary>
    /// Initializes a level with no neighbors or adjacency rules and no tile options for any cells
    /// Use <see cref="LevelTestExtensions"/> to populate level neighbors, rules and options
    /// </summary>
    /// <param name="cellCount"></param>
    /// <param name="tileTypeCount"></param>
    /// <returns></returns>
    internal static Level CreateDummyLevelWithUnitWeights(int cellCount, int tileTypeCount)
    {
        double[] weights = Enumerable.Repeat(1.0, tileTypeCount).ToArray();
        return CreateDummyLevel(cellCount, weights);
    }

    internal static Level CreateDummyLevel(int cellCount, double[] weights)
    {
        var tileTypeCount = weights.Length;
       
        TileRules[] rules = new TileRules[tileTypeCount];
        for (int i = 0; i < tileTypeCount; i++)
        {
            rules[i] = new TileRules(new()
            {
                { Direction.North, new BitArray(tileTypeCount) },
                { Direction.East, new BitArray(tileTypeCount) },
                { Direction.South, new BitArray(tileTypeCount) },
                { Direction.West, new BitArray(tileTypeCount) },
            });
        }
        
        TileType[] tileTypes = new TileType[tileTypeCount];
        for (int i = 0; i < tileTypes.Length; i++)
        {
            tileTypes[i] = new TileType($"{i}");
        }
        
        // Init cell arrays
        Vector[] position = new Vector[cellCount];
        bool[] collapsed = new bool[cellCount];
        
        BitArray[] options = new BitArray[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            options[i] = new BitArray(tileTypeCount, true);
        }

        Neighbors[] neighborIndices = new Neighbors[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            neighborIndices[i] = new Neighbors(new());
        }
        
        double sumOfWeightsLogWeightsNumber = weights.Sum(w => Math.Log2(w) * w);
        double sumOfWeightsNumber = weights.Sum();
        double[] sumOfWeights = Enumerable.Repeat(sumOfWeightsNumber, cellCount).ToArray();
        double[] sumOfWeightsLogWeights = 
            Enumerable.Repeat(sumOfWeightsLogWeightsNumber, cellCount).ToArray();

        double[] entropy = 
            Enumerable.Repeat(Math.Log2(sumOfWeightsNumber) - sumOfWeightsLogWeightsNumber / sumOfWeightsNumber, cellCount).ToArray();
        
        return new Level(
            rules: rules,
            tileTypes: tileTypes,
            position: position,
            options: options,
            entropy: entropy,
            neighborIndices: neighborIndices,
            collapsed: collapsed,
            totalTileTypeCount: tileTypeCount,
            weights: weights,
            sumOfWeights: sumOfWeights,
            sumOfWeightsLogWeights: sumOfWeightsLogWeights
        );
    }
}