using BenchmarkDotNet.Attributes;
using Domain.Models;
using Models;
using WFC;
using WFC.Args;
using WFC.Models;
using WFC.Tests;

namespace Benchmarks;

[MemoryDiagnoser]
public class OptionsBenchmarker
{
    [Params(10, 50, 100)] public int Dimension;

    private const string TilemapPath =
        "/Users/dkWiSkHe/RiderProjects/masters-thesis/UnityMastersThesis/Assets/Resources/Tiles/Pokemon/Tilemaps/PalletTown.png";

    private State _startingState;

    [IterationSetup]
    public void IterationSetup()
    {
        Level level = LevelFactory.Create(Dimension, TilemapPath, 5);
        _startingState = new State(level);
    }

    [Benchmark]
    public State CompleteWfc()
    {
        return WaveFunctionCollapse.Complete(_startingState);
    }
    
}