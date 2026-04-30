using BenchmarkDotNet.Attributes;
using WFC;
using WFC.Models;
using WFC.Tests.Helpers;

namespace Benchmarks;

[MemoryDiagnoser]
public class OptionsBenchmarker
{
    [Params(10, 50, 100)] public int Dimension;

    [Params(5, 10, 20)] public int PropagationDepth;
    
    private const string TilemapPath =
        "/Users/dkWiSkHe/RiderProjects/masters-thesis/UnityMastersThesis/Assets/Resources/Tiles/Pokemon/Tilemaps/PalletTown.png";

    private State _startingState;

    [IterationSetup]
    public void IterationSetup()
    {
        Level level = LevelFactory.Create(Dimension, TilemapPath, PropagationDepth);
        _startingState = new State(level, 0);
    }

    [Benchmark]
    public State CompleteWfc()
    {
        return WaveFunctionCollapse.Complete(_startingState);
    }
}