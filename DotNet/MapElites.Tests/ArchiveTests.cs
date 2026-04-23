using MapElites.Models;

namespace MapElites.Tests;

public class Tests
{
    public class TestKey(int keyIndex) : BaseKey<TestKey>
    {
        private int KeyIndex { get; } = keyIndex;

        public override bool Equals(TestKey? other) => other?.KeyIndex == KeyIndex;
        public override int GetHashCode() => HashCode.Combine(KeyIndex);
    }
    
    [Test]
    public void SamplingArchive_WithOneEntry_ReturnsTheEntry()
    {
        // Arrange
        Archive<TestKey, SampleEntry, SampleIndividual, SampleBehavior> archive = new(1);
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entry = new(individual, new SampleBehavior(), 0);
        archive.TryAdd(new TestKey(0), entry);

        // Act 
        SampleIndividual sampledIndividual = archive.Sample();

        // Assert
        Assert.That(sampledIndividual, Is.EqualTo(individual));
    }

    [Test]
    public void SamplingArchive_WithZeroEntries_ThrowsException()
    {
        // Arrange
        Archive<TestKey, SampleEntry, SampleIndividual, SampleBehavior> archive = new(1);

        // Act 
        Func<SampleIndividual> sample = archive.Sample;

        // Assert
        Assert.That(sample, Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TryAddingEntry_ToEmptyArchive_WillSucceed()
    {
        // Arrange
        Archive<TestKey, SampleEntry, SampleIndividual, SampleBehavior> archive = new(1);
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entry = new(individual, new SampleBehavior(), 0);

        // Act 
        bool success = archive.TryAdd(new TestKey(0), entry);

        // Assert
        Assert.That(success, Is.True);
    }

    [Test]
    public void TryAddingEntry_ThatAlreadyExistsWithBetterFitness_WillFail()
    {
        // Arrange
        Archive<TestKey, SampleEntry, SampleIndividual, SampleBehavior> archive = new(2);
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entryHighFitness = new(individual, new SampleBehavior(), 10);
        archive.TryAdd(new TestKey(0), entryHighFitness);
        SampleEntry entryLowFitness = new(individual, new SampleBehavior(), 0);

        // Act 
        bool success = archive.TryAdd(new TestKey(0), entryLowFitness);

        // Assert
        Assert.That(success, Is.False);
    }

    [Test]
    public void TryAddingEntry_ThatExistsWithLowerFitness_WillSucceed()
    {
        // Arrange
        Archive<TestKey, SampleEntry, SampleIndividual, SampleBehavior> archive = new(2);
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entryLowFitness = new(individual, new SampleBehavior(), 0);
        archive.TryAdd(new TestKey(0), entryLowFitness);
        SampleEntry entryHighFitness = new(individual, new SampleBehavior(), 10);

        // Act 
        bool success = archive.TryAdd(new TestKey(0), entryHighFitness);

        // Assert
        Assert.That(success, Is.True);
    }
}