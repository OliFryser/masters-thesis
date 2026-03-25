using Domain.Models;
using WFC.Extensions;

namespace WFC.Tests.Unit.WfcArgsExtensionTests;

[TestFixture]
public class ToLevelTests
{
    [Test]
    public void CreateNeighborsArrayReturnsCorrectNeighborIndices()
    {
        // Arrange
        var basePosition = new Vector(1, 1);
        var northPosition = new Vector(1, 2);
        var southPosition = new Vector(1, 0);
        var westPosition = new Vector(0, 1);
        var eastPosition = new Vector(2, 1);
        Vector[] positions = [basePosition, northPosition, southPosition, westPosition, eastPosition];

        // Act
        var neighbors = WfcArgsExtensions.CreateNeighborsArray(positions);

        // Assert
        Assert.That(neighbors.Length, Is.EqualTo(5), "Every position can have neighbors");
        Assert.That(neighbors[0].Indices[Direction.North], Is.EqualTo(1));
        Assert.That(neighbors[0].Indices[Direction.South], Is.EqualTo(2));
        Assert.That(neighbors[0].Indices[Direction.West], Is.EqualTo(3));
        Assert.That(neighbors[0].Indices[Direction.East], Is.EqualTo(4));
    }
}