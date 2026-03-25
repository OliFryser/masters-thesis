using System.Collections;
using Domain.Models;
using WFC.Extensions;
using WFC.Models;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit.LevelExtensionTests;

[TestFixture]
public class AccumulateValidTilesTests
{
    [Test]
    public void ValidTilesForDirectionAreAccumulated_SingleValidTile()
    {
        // Arrange
        var cellCount = 2;
        var tileCount = 2;
        Level level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount, tileCount);
        level.AddNeighborConnection(0, 1, Direction.North);
        level.AddAdjacencyRule(0, 1, Direction.North);

        var validNeighbors = new BitArray(cellCount, true);
        
        // Act
        level.AccumulateValidTilesInDirection(validNeighbors, 1, Direction.North);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validNeighbors[0], Is.True, "Tile 0 should be valid for north neighbor");
            Assert.That(validNeighbors[1], Is.False, "Tile 1 should be invalid for north neighbor");
        }
    }
    
    [Test]
    public void ValidTilesForDirectionAreAccumulated_MultipleValidTiles()
    {
        // Arrange
        var cellCount = 2;
        var tileCount = 2;
        Level level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount, tileCount);
        level.AddNeighborConnection(0, 1, Direction.North);
        level.AddAdjacencyRule(0, 1, Direction.North);
        level.AddAdjacencyRule(1, 0, Direction.North);

        var validNeighbors = new BitArray(cellCount, true);
        
        // Act
        level.AccumulateValidTilesInDirection(validNeighbors, 1, Direction.North);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validNeighbors[0], Is.True, "Tile 0 should be valid for north neighbor");
            Assert.That(validNeighbors[1], Is.True, "Tile 1 should be invalid for north neighbor");
        }
    }
    
    [Test]
    public void ValidTilesForDirectionAreAccumulated_NoValidTiles()
    {
        // Arrange
        var cellCount = 2;
        var tileCount = 2;
        Level level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount, tileCount);
        level.AddNeighborConnection(0, 1, Direction.North);

        var validNeighbors = new BitArray(cellCount, true);
        
        // Act
        level.AccumulateValidTilesInDirection(validNeighbors, 1, Direction.North);
        
        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(validNeighbors[0], Is.False, "Tile 0 should be valid for north neighbor");
            Assert.That(validNeighbors[1], Is.False, "Tile 1 should be invalid for north neighbor");
        }
    }
    
}