using System.Collections;
using WFC.Extensions;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit.LevelExtensionTests;

[TestFixture]
public class ReduceEntropyTests
{
    [Test]
    public void ReducesEntropyForUncollapsedCell()
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount: 1, 2);
        level.UpdateSumOfWeights(0, new BitArray([false, true]));
        var expectedEntropy =
            EntropyCalculation.CalculateEntropy(level.SumOfWeights[0], level.SumOfWeightsLogWeights[0]);
        
        // Act
        level.ReduceEntropy(0);
        
        // Assert
        Assert.That(level.Entropy[0], Is.EqualTo(expectedEntropy), "Entropy should not change for collapsed cells.");
    }
}