using System.Collections;
using WFC.Extensions;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit.LevelExtensionTests;

[TestFixture]
public class UpdateSumOfWeightsTests
{
    [Test]
    public void GivenNoExcludedOptions_DoesNotChangeWeightsAndSumOfWeights()
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
    
    [TestCase(new int[] { 0 })]
    [TestCase(new int[] { 1 })]
    [TestCase(new int[] { 2 })]
    public void GivenExcludedOptions_ChangesSumOfWeightsByWeightOfOption(int[] weights)
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevel(1, weights);
        var sumOfWeightsBefore = level.SumOfWeights[0];
        var sumOfWeightsLogWeightsBefore = level.SumOfWeightsLogWeights[0];
        
        // Act
        level.UpdateSumOfWeights(0, new BitArray([true]));
        
        // Assert
        var expectedSumOfWeights = sumOfWeightsBefore - level.Weights[0];
        var expectedSumOfWeightsLogWeights = sumOfWeightsLogWeightsBefore - MathF.Log2(level.Weights[0]) * level.Weights[0];
        Assert.That(level.SumOfWeights[0], Is.EqualTo(expectedSumOfWeights));
        Assert.That(level.SumOfWeightsLogWeights[0], Is.EqualTo(expectedSumOfWeightsLogWeights));
    }
}