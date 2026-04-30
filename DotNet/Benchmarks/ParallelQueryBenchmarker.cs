using BenchmarkDotNet.Attributes;
using Domain.Models;
using Pokémon;
using TilemapAnalysis;
using WFC;
using WFC.Args;
using WFC.Models;

namespace Benchmarks;

[MemoryDiagnoser]
public class ParallelQueryBenchmarker
{
    [Params(1, 10, 50, 1000)] public int EvaluationIterations;

    public List<Vector> Coordinates { get; set; }
    public IReadOnlyCollection<TileType> TileTypes { get; set; }
    public IReadOnlyCollection<AdjacencyRule> AdjacencyRules { get; set; }
    public List<TileWeight> Weights { get; set; }

    public static string BaseDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}/../../../../../../..";
    private static string ResourceDirectory => $"{BaseDirectory}/Resources";
    public static string TilemapPath => $"{ResourceDirectory}/Tilemaps/{TilemapName}";
    public static string TilemapName => "PalletTown.png";

    
    [GlobalSetup]
    public void GlobalSetup()
    {
        using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(TilemapPath);
        TileTypes = tilemapAnalyzer.Tiles.Select(t => t.Type).ToHashSet().ToList();
        int tileTypeCount = tilemapAnalyzer.TileTypeCount;
        AdjacencyRules = tilemapAnalyzer.GetAdjacencyRules();

        int mapDimension = 20;
        Coordinates = LevelGeneration.GetRectangleCoordinates(mapDimension, mapDimension).ToList();

        Random random = new Random();
        Weights = TileTypes.Select(t => new TileWeight(t, random.Next(tileTypeCount))).ToList();
    }

    [Benchmark(Baseline = true)]
    public State[] RunSingleThreaded()
    {
        return Enumerable.Range(0, EvaluationIterations)
            .Select(i =>
            {
                WfcArgs args = new WfcArgs(Coordinates, TileTypes, AdjacencyRules, Weights, i);
                State state = WaveFunctionCollapse.Run(args);
                return state;
            })
            .ToArray();
    }


    [Benchmark]
    public State[] RunParallel()
    {
        return Enumerable.Range(0, EvaluationIterations)
            .AsParallel()
            .Select(i =>
            {
                WfcArgs args = new WfcArgs(Coordinates, TileTypes, AdjacencyRules, Weights, i);
                State state = WaveFunctionCollapse.Run(args);
                return state;
            })
            .ToArray();
    }
}