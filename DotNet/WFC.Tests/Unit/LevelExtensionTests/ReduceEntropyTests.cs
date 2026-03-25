using Domain.Models;
using WFC.Extensions;
using WFC.Tests.Helpers;

namespace WFC.Tests.Unit.LevelExtensionTests;

[TestFixture]
public class ReduceEntropyTests
{
    [Test]
    public void ReduceEntropy_DoesNothing_IfCellIsAlreadyCollapsed()
    {
        // Arrange
        var level = LevelFactory.CreateDummyLevelWithUnitWeights(cellCount: 1, 1);
        level.Collapsed[0] = true;
        float initialEntropy = 10f;
        level.Entropy[0] = initialEntropy;

        // Act
        level.ReduceEntropy(0);

        // Assert
        Assert.That(level.Entropy[0], Is.EqualTo(initialEntropy), "Entropy should not change for collapsed cells.");
    }
}