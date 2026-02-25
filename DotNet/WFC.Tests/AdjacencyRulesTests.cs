using WFC.Models;

namespace WFC.Tests;

public class AdjacencyRulesTests
{
    [Test]
    public void AdjacencyRules_ReturnsEmptyCollection_ForUnknownType()
    {
        // Arrange
        var adjacencyRules = new AdjacencyRules();
        var dummyTileType = new TileType("Dummy");
        
        // act
        var northRules = adjacencyRules.GetAdjacencyRules(Direction.North, dummyTileType);
        var southRules = adjacencyRules.GetAdjacencyRules(Direction.South, dummyTileType);
        var eastRules = adjacencyRules.GetAdjacencyRules(Direction.East, dummyTileType);
        var westRules = adjacencyRules.GetAdjacencyRules(Direction.West, dummyTileType);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(northRules, Is.Empty);
            Assert.That(southRules, Is.Empty);
            Assert.That(eastRules, Is.Empty);
            Assert.That(westRules, Is.Empty);
        }
    }
    
    [Test]
    public void AdjacencyRules_ReturnsCollectionOfRules_ForKnownTileType()
    {
        // Arrange
        var adjacencyRules = new AdjacencyRules();
        var waterTile =  new TileType("Water");
        var tileType =  new TileType("Sand");
    }
}