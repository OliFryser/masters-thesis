using Domain.Models;
using WFC.Extensions;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit.LevelExtensionTests;

[TestFixture]
public class PruneInconsistentOptionsTests
{
    [Test]
    public void PrunesOptionsBasedOnNeighbors()
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount: 2, tileTypeCount: 2);
        
        level.AddNeighborConnection(0, 1, Direction.North);
        
        // Tile 0 can be adjacent with tile 1 to the north
        level.AddAdjacencyRule(0, 1, Direction.North);
        
        level.ChooseOptionForCell(cell: 0, tile: 0);
        
        // Act
        level.PruneInconsistentOptions(1);

        // Assert
        var optionsForCell1 = level.GetOptionsForCell(1);
        
        
        Assert.That(optionsForCell1[0], Is.False, "Tile 0 should be pruned from Cell 1.");
        Assert.That(optionsForCell1[1], Is.True, "Tile 1 should remain valid in Cell 1.");
    }
}