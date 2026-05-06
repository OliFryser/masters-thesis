using Domain.Models;
using Pokémon.Calculations;

namespace Pokémon.Tests;

public class CalculateTests
{
    [Test]
    public void Variation_ReturnsZero_WhenAllTilesHaveSameType()
    {
        var tiles = CreateTiles(("grass", 20));

        var variation = Calculate.Variation(tiles, tileTypeCount: 4);

        Assert.That(variation, Is.EqualTo(0f).Within(1e-6f));
    }

    [Test]
    public void Variation_ReturnsOne_WhenDistributionIsUniformAcrossAllTileTypes()
    {
        var tiles = CreateTiles(("grass", 10), ("water", 10), ("rock", 10), ("sand", 10));

        var variation = Calculate.Variation(tiles, tileTypeCount: 4);

        Assert.That(variation, Is.EqualTo(1f).Within(1e-6f));
    }

    [Test]
    public void Variation_IsWithinRangeZeroToOne_ForSkewedValidDistribution()
    {
        var tiles = CreateTiles(("grass", 14), ("water", 4), ("rock", 2));

        var variation = Calculate.Variation(tiles, tileTypeCount: 3);

        Assert.That(variation, Is.GreaterThanOrEqualTo(0f));
        Assert.That(variation, Is.LessThanOrEqualTo(1f));
    }

    [Test]
    public void Variation_RemainsWithinRange_ForManyRandomValidInputs()
    {
        var random = new Random(12345);

        for (var i = 0; i < 250; i++)
        {
            var tileTypeCount = random.Next(2, 13);
            var uniqueTypes = random.Next(1, tileTypeCount + 1);

            var counts = Enumerable.Range(0, uniqueTypes)
                .Select(_ => random.Next(1, 40))
                .ToArray();

            var tiles = CreateTiles(counts
                .Select((count, idx) => ($"type-{idx}", count))
                .ToArray());

            var variation = Calculate.Variation(tiles, tileTypeCount);

            Assert.That(variation, Is.GreaterThanOrEqualTo(0f), $"Iteration {i} produced {variation}");
            Assert.That(variation, Is.LessThanOrEqualTo(1f), $"Iteration {i} produced {variation}");
        }
    }
    
    private static List<Tile> CreateTiles(params (string type, int count)[] buckets)
    {
        var tiles = new List<Tile>();
        var x = 0;

        foreach (var (type, count) in buckets)
        {
            for (var i = 0; i < count; i++)
            {
                tiles.Add(new Tile(x, 0, type));
                x++;
            }
        }

        return tiles;
    }
}