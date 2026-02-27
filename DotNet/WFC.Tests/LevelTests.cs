namespace Domain.Tests;

public class Tests
{
    // [Test]
    // public void Level_Throws_OnDuplicateCellLocations()
    // {
    //     // Arrange
    //     List<Cell> cells = new List<Cell>
    //     {
    //         new Cell(new Vector(0,0), new HashSet<TileOption>()),
    //         new Cell(new Vector(0,0), new HashSet<TileOption>()),
    //     };
    //     Level MakeLevel() => new Level(cells);
    //
    //     // Act
    //
    //     // Assert
    //     Assert.That(MakeLevel, Throws.TypeOf<ArgumentException>());
    // }
    //
    // [Test]
    // public void Level_Has_CellLookupByPosition()
    // {
    //     // Arrange
    //     Cell cell00 = new Cell(new Vector(0, 0), new HashSet<TileOption>());
    //     Cell cell10 = new Cell(new Vector(1, 0), new HashSet<TileOption>()); 
    //     List<Cell> cells =
    //     [
    //         cell00,
    //         cell10
    //     ];
    //
    //     // Act
    //     var level = new Level(cells);
    //
    //     Assert.Multiple(() =>
    //     {
    //         // Assert
    //         Assert.That(level.Cells, Has.Count.EqualTo(2));
    //         Assert.That(level.CellLookup[new Vector(0, 0)], Is.EqualTo(cell00));
    //     });
    // }
}