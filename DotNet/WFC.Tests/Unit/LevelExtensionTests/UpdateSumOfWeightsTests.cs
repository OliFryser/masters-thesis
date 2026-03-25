using System.Collections;
using WFC.Extensions;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit.LevelExtensionTests;

[TestFixture]
public class UpdateSumOfWeightsTests
{
    [Test]
    public static void GivenNoExcludedOptions_DoesNotChangeWeightsAndSumOfWeights()
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevelWithUnitWeights(1, 1);
        var sumOfWeightsBefore = level.SumOfWeights[0];
        var sumOfWeightsLogWeightsBefore = level.SumOfWeightsLogWeights[0];

        // Act
        level.UpdateSumOfWeights(0, new BitArray(1));

        // Assert
        Assert.That(level.SumOfWeights[0], Is.EqualTo(sumOfWeightsBefore));
        Assert.That(level.SumOfWeightsLogWeights[0], Is.EqualTo(sumOfWeightsLogWeightsBefore));
    }
}