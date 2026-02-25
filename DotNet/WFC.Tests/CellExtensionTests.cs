using Domain;
using WFC.Args;
using WFC.Extensions;
using WFC.Models;

namespace WFC.Tests;

public class Tests
{
    private Cell _cellWithZeroOptions;
    private Cell _cellWithOneOption;
    private Cell _cellWithManyOptions;

    [SetUp]
    public void Setup()
    {
        _cellWithZeroOptions = GetCellWithZeroOptions();
        _cellWithOneOption = GetCellWithOneOption();
        _cellWithManyOptions = GetCellWithManyOptions();
    }
    
    private Cell GetCellWithZeroOptions()
    {
        Vector position = new(0, 0);
        HashSet<TileOption> options = [];
        Cell cell = new(position, options);
        return cell;
    }
    
    private Cell GetCellWithOneOption()
    {
        Vector position = new(0, 0);
        
        TileOption option = new()
        {
            // TileType = new TileType
            // {
            //     Type = "Grass"
            // }
        };
        
        HashSet<TileOption> options = [option];
        
        Cell cell = new(position, options);
        
        return cell;
    }
    
    private Cell GetCellWithManyOptions()
    {
        Vector position = new(0, 0);
        
        TileOption option = new()
        {
            // TileType = new TileType
            // {
            //     Type = "Grass"
            // }
        };
        
        TileOption option2 = new()
        {
            // TileType = new TileType
            // {
            //     Type = "Water"
            // }
        };

        HashSet<TileOption> options = [option, option2];
        
        Cell cell = new(position, options);

        return cell;
    }

    [Test]
    public void Cell_Without_Options_Is_Not_Collapsable()
    {
        // Arrange

        // Act
        bool isCollapsable = _cellWithZeroOptions.IsCollapsable();
        
        // Assert
        Assert.That(isCollapsable, Is.False);
    }

    [Test]
    public void Cell_With_One_Option_Is_Collapsable()
    {
        // Arrange
        
        // Act
        bool isCollapsable = _cellWithOneOption.IsCollapsable();
        
        // Assert
        Assert.That(isCollapsable, Is.True);
    }
    
    [Test]
    public void Cell_With_Many_Options_Is_Collapsable()
    {
        // Arrange

        // Act
        bool isCollapsable = _cellWithManyOptions.IsCollapsable();
        
        // Assert
        Assert.That(isCollapsable, Is.True);
    }
    
    [Test]
    public void Collapse_Cell_Leaves_Only_One_Option()
    {
        // Arrange
        
        // Act
        Cell collapsedCell = _cellWithManyOptions.CollapseCell();
        
        // Assert
        Assert.That(collapsedCell.IsCollapsed(), Is.True);
    }

    [Test]
    public void Collapsing_A_Cell_With_No_Options_Throws()
    {
        // Arrange
        Func<Cell> collapseAction = () => _cellWithZeroOptions.CollapseCell();
        
        // Act
        
        // Assert
        Assert.That(collapseAction, Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Cell_With_Zero_Options_Is_Not_Collapsed()
    {
        // Act
        bool isCollapsed = _cellWithZeroOptions.IsCollapsed();
        
        // Assert
        Assert.That(isCollapsed, Is.False);
    }

    [Test]
    public void Cell_With_One_Option_Is_Collapsed()
    {
        // Act
        bool isCollapsed = _cellWithOneOption.IsCollapsed();
        
        // Assert
        Assert.That(isCollapsed, Is.True);
    }

    [Test]
    public void Cell_With_Many_Options_Is_Not_Collapsed()
    {
        // Act
        bool isCollapsed = _cellWithManyOptions.IsCollapsed();
        
        // Arrange
        Assert.That(isCollapsed, Is.False);
    }

    [Test]
    public void Option_With_High_Weight_Is_Chosen_Most_Often()
    {
        
    }
}