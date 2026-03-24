namespace WFC.Tests.Unit;

[TestFixture]
public class CalculateEntropyTests
{
    [TestCase(0)]
    [TestCase(-1)]
    public static void CalculateEntropy_GivenSumOfWeightsZeroOrBelowZero_ReturnsZero(int sumOfWeights)
    {
        // Arrange
        float sumOfWeightsLogWeights = 1f;

        // Act
        var actual = WFC.EntropyCalculation.CalculateEntropy(sumOfWeights, sumOfWeightsLogWeights);

        // Assert
        Assert.That(actual, Is.Zero);
    }
    
    [Test]
    public static void CalculateEntropy_ReturnsCorrectShannonEntropy()
    {
        // Arrange
        int[] weights = [1, 1, 1];

        var sumOfWeights = weights.Sum();
        var sumOfWeightsLogWeight =  weights.Sum(w => w * MathF.Log2(x: sumOfWeights));
            
        // Act
        var actual = EntropyCalculation.CalculateEntropy(sumOfWeights: sumOfWeights, sumOfWeightsLogWeight: sumOfWeightsLogWeight);

        // Assert
        var expected = MathF.Log2(x: weights.Sum()) - sumOfWeightsLogWeight / sumOfWeights;
        Assert.That(actual: actual, expression: Is.EqualTo(expected));
    }
}