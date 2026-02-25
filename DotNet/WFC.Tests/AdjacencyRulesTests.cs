namespace WFC.Tests;

public class AdjacencyRulesTests
{
    // [Test]
    // public void AdjacencyRules_ReturnsEmptyCollection_ForUnknownType()
    // {
    //     // Arrange
    //     var adjacencyRules = new AdjacencyRules();
    //     var dummyTileType = new TileType(0);
    //     
    //     // act
    //     var northRules = adjacencyRules.GetAdjacencyRules(Direction.North, dummyTileType);
    //     var southRules = adjacencyRules.GetAdjacencyRules(Direction.South, dummyTileType);
    //     var eastRules = adjacencyRules.GetAdjacencyRules(Direction.East, dummyTileType);
    //     var westRules = adjacencyRules.GetAdjacencyRules(Direction.West, dummyTileType);
    //     using (Assert.EnterMultipleScope())
    //     {
    //         Assert.That(northRules, Is.Empty);
    //         Assert.That(southRules, Is.Empty);
    //         Assert.That(eastRules, Is.Empty);
    //         Assert.That(westRules, Is.Empty);
    //     }
    // }
    //
    // [Test]
    // public void AdjacencyRules_ReturnsCollectionOfRules_ForKnownTileType()
    // {
    //     // Arrange
    //     var adjacencyRules = new AdjacencyRules();
    //     var waterTile =  new TileType(0);
    //     var sandTile =  new TileType(1);
    //     var adjacencyRule = new AdjacencyRule(waterTile, sandTile, Direction.North, 1);
    //     adjacencyRules.AddRule(Direction.North, waterTile, adjacencyRule);
    //     
    //     // Act
    //     var rules = adjacencyRules.GetAdjacencyRules(Direction.North, waterTile);
    //     
    //     // Assert
    //     Assert.That(rules, Has.Count.EqualTo(1));
    //     Assert.That(rules.First(), Is.EqualTo(adjacencyRule));
    // }
}