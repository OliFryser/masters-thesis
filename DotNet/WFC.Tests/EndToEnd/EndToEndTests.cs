using Domain.Models;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;
using WFC.Output;
using WFC.Tests.Helpers;

namespace WFC.Tests.EndToEnd;

[TestFixture]
public class EndToEndTests
{
    private static readonly string ResourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../Resources/";
    private static readonly string TilemapPath =
        $"{ResourceDirectory}/Tilemaps/PalletTown.png";


    [Test]
    public void Wfc_Completes_WithOnlyOneCell()
    {
        TileType tile = new TileType("0");
        IReadOnlyCollection<Vector> coordinates = [new Vector(0, 0)];
        IReadOnlyCollection<TileType> tileTypes = [tile];
        IReadOnlyCollection<AdjacencyRule> adjacencyRules = [];
        IReadOnlyDictionary<TileType, int> tileTypeToCount = new Dictionary<TileType, int>
        {
            { tile, 1 },
        };
        int tileCount = 1;
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, tileTypeToCount, tileCount);
        Result result = WaveFunctionCollapse.Run(args);

        Assert.That(result, Is.Not.Null);

        Assert.That(result.Status.Success, Is.True);
    }

    [Test]
    public void Wfc_Completes_WithTwoCells()
    {
        TileType tile0 = new TileType("0");
        TileType tile1 = new TileType("1");
        IReadOnlyCollection<Vector> coordinates = [new Vector(0, 0), new Vector(1, 0)];
        IReadOnlyCollection<TileType> tileTypes = [tile0, tile1];
        IReadOnlyCollection<AdjacencyRule> adjacencyRules =
        [
            new AdjacencyRule(tile0, tile1, Direction.East),
            new AdjacencyRule(tile0, tile1, Direction.West),
            new AdjacencyRule(tile1, tile0, Direction.East),
            new AdjacencyRule(tile1, tile0, Direction.West)
        ];
        IReadOnlyDictionary<TileType, int> tileTypeToCount = new Dictionary<TileType, int>
        {
            { tile0, 2 },
            { tile1, 2 },
        };
        int tileCount = 4;
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, tileTypeToCount, tileCount);

        Result result = WaveFunctionCollapse.Run(args);

        Assert.That(result, Is.Not.Null);

        Assert.That(result.Status.Success, Is.True);
    }

    [Test]
    public void Wfc_DoesNotComplete_WithInfeasibleRules()
    {
        TileType tile0 = new TileType("0");
        TileType tile1 = new TileType("1");
        IReadOnlyCollection<Vector> coordinates = [new Vector(0, 0), new Vector(1, 0)];
        IReadOnlyCollection<TileType> tileTypes = [tile0, tile1];
        IReadOnlyCollection<AdjacencyRule> adjacencyRules = [];
        IReadOnlyDictionary<TileType, int> tileTypeToCount = new Dictionary<TileType, int>
        {
            { tile0, 1 },
            { tile1, 1 },
        };
        int tileCount = 2;
        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, tileTypeToCount, tileCount);
        Result result = WaveFunctionCollapse.Run(args);

        Assert.That(result, Is.Not.Null);

        Assert.That(result.Status.Success, Is.False);
    }

    [Test]
    public void Wfc_Completes_InTwoSteps()
    {
        TileType tile0 = new TileType("0");
        TileType tile1 = new TileType("1");
        IReadOnlyCollection<Vector> coordinates = [new Vector(0, 0), new Vector(1, 0)];
        IReadOnlyCollection<TileType> tileTypes = [tile0, tile1];
        IReadOnlyCollection<AdjacencyRule> adjacencyRules =
        [
            new AdjacencyRule(tile0, tile1, Direction.East),
            new AdjacencyRule(tile0, tile1, Direction.West),
            new AdjacencyRule(tile1, tile0, Direction.East),
            new AdjacencyRule(tile1, tile0, Direction.West)
        ];
        
        IReadOnlyDictionary<TileType, int> tileTypeToCount = new Dictionary<TileType, int>
        {
            { tile0, 2 },
            { tile1, 2 },
        };
        int tileCount = 4;

        WfcArgs args = new WfcArgs(coordinates, tileTypes, adjacencyRules, tileTypeToCount, tileCount);
        
        State step0 = args.ToState();
        State step1 = WaveFunctionCollapse.Step(step0);
        State step2 = WaveFunctionCollapse.Step(step1);

        Assert.That(step2.IsCollapsed, Is.True);
    }

    [Test]
    public void Wfc_CanCompleteSmall_FromInputTilemap()
    {
        Level level = LevelFactory.Create(10, TilemapPath, 5);
        State startingPoint = new State(level);
        State state = WaveFunctionCollapse.Complete(startingPoint);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(level.CanStep(), Is.False);
        }
    }

    [Test]
    public void Wfc_CanCompleteMedium_FromInputTilemap()
    {
       Level level = LevelFactory.Create(50, TilemapPath, 5);
        State startingPoint = new State(level);
        State state = WaveFunctionCollapse.Complete(startingPoint);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(level.CanStep(), Is.False);
        }
    }

    [Test]
    public void Wfc_CanCompleteLarge_FromInputTilemap()
    {
        Level level = LevelFactory.Create(100, TilemapPath, 5);
        State startingPoint = new State(level);
        State state = WaveFunctionCollapse.Complete(startingPoint);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(state, Is.Not.Null);
            Assert.That(level.CanStep(), Is.False);
        }
    }
}