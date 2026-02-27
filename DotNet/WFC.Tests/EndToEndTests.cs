using Domain.Models;
using WFC.Args;
using WFC.Output;

namespace WFC.Tests;

public class EndToEndTests
{
    [Test]
    public void Wfc_Completes()
    {
        TileType tile = new TileType(0);
        IReadOnlyCollection<Vector> coordinates = [new Vector(0, 0)];
        IReadOnlyCollection<TileType> tileTypes = [tile];
        IReadOnlyCollection<AdjacencyRule> adjacencyRules = [];
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules);
        Result result = WaveFunctionCollapse.Run(args);
        
        Assert.That(result, Is.Not.Null);
        
        Assert.That(result.Status.Success, Is.True);
    }
}