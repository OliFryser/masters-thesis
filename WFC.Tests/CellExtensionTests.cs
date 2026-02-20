using Domain;
using WFC.Extensions;

namespace WFC.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Cell_Without_Options_Is_Not_Collapsable()
    {
        // Arrange
        Vector position = new(0, 0);
        HashSet<TileOption> options = [];
        Cell cell = new(position, options);

        // Act
        bool isCollapsable = cell.IsCollapsable();
        
        // Assert
        Assert.That(isCollapsable, Is.False);
    }

    [Test]
    public void Cell_With_One_Option_Is_Collapsable()
    {
        // Arrange
        Vector position = new(0, 0);
        
        TileOption option = new()
        {
            TileType = new TileType
            {
                Type = "Grass"
            }
        };
        
        HashSet<TileOption> options = [option];
        
        Cell cell = new(position, options);

        // Act
        bool isCollapsable = cell.IsCollapsable();
        
        // Assert
        Assert.That(isCollapsable, Is.True);
    }
    
    [Test]
    public void Cell_With_Many_Options_Is_Collapsable()
    {
        // Arrange
        Vector position = new(0, 0);
        
        TileOption option = new()
        {
            TileType = new TileType
            {
                Type = "Grass"
            }
        };
        
        TileOption option2 = new()
        {
            TileType = new TileType
            {
                Type = "Water"
            }
        };

        HashSet<TileOption> options = [option, option];
        
        Cell cell = new(position, options);

        // Act
        bool isCollapsable = cell.IsCollapsable();
        
        // Assert
        Assert.That(isCollapsable, Is.True);
    }
}