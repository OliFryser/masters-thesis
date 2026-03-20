using System.Collections;
using Domain.Models;
using WFC.Extensions;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit;

[TestFixture]
public class ReduceEntropyTests
{
    [Test]
    public void ReduceEntropy_DoesNothing_IfCellIsAlreadyCollapsed()
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount: 1, 1);
        level.Collapsed[0] = true;
        float initialEntropy = 10f;
        level.Entropy[0] = initialEntropy;

        // Act
        level.ReduceEntropy(0);

        // Assert
        Assert.That(level.Entropy[0], Is.EqualTo(initialEntropy), "Entropy should not change for collapsed cells.");
    }

    [Test]
    public void ReduceEntropy_FiltersOptionsBasedOnNeighbors()
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount: 2, tileTypeCount: 2);
        
        level.AddNeighborConnection(0, 1, Direction.North);
        
        // Tile 0 can be adjacent with tile 1 to the north
        level.AddAdjacencyRule(0, 1, Direction.North);
        
        level.ChooseOptionForCell(cell: 0, tile: 0);
        
        // Act
        level.ReduceEntropy(1);

        // Assert
        var optionsForCell1 = level.GetOptionsForCell(1);
        
        
        Assert.That(optionsForCell1[0], Is.False, "Tile 0 should be pruned from Cell 1.");
        Assert.That(optionsForCell1[1], Is.True, "Tile 1 should remain valid in Cell 1.");
    }
}