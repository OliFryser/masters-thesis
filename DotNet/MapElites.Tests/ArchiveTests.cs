using MapElites.Models;

namespace MapElites.Tests;

public class Tests
{
    [Test]
    public void SamplingArchive_WithOneEntry_ReturnsTheEntry()
    {
        // Arrange
        Archive<SampleIndividual, SampleBehavior, int> archive = new();
        SampleIndividual individual = new SampleIndividual();
        Entry<SampleIndividual, SampleBehavior> entry = new(individual, new SampleBehavior(), 0);
        archive.TryAdd(0, entry);

        // Act 
        SampleIndividual sampledIndividual = archive.SampleRandom();

        // Assert
        Assert.That(sampledIndividual, Is.EqualTo(individual));
    }

    [Test]
    public void SamplingArchive_WithZeroEntries_ThrowsException()
    {
        // Arrange
        Archive<SampleIndividual, SampleBehavior, int> archive = new();

        // Act 
        Func<SampleIndividual> sample = archive.SampleRandom;

        // Assert
        Assert.That(sample, Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TryAddingEntry_ToEmptyArchive_WillSucceed()
    {
        // Arrange
        Archive<SampleIndividual, SampleBehavior, int> archive = new();
        SampleIndividual individual = new SampleIndividual();
        Entry<SampleIndividual, SampleBehavior> entry = new(individual, new SampleBehavior(), 0);

        // Act 
        bool success = archive.TryAdd(0, entry);

        // Assert
        Assert.That(success, Is.True);
    }

    [Test]
    public void TryAddingEntry_ThatAlreadyExistsWithBetterFitness_WillFail()
    {
        // Arrange
        Archive<SampleIndividual, SampleBehavior, int> archive = new();
        SampleIndividual individual = new SampleIndividual();
        Entry<SampleIndividual, SampleBehavior> entryHighFitness = new(individual, new SampleBehavior(), 10);
        archive.TryAdd(0, entryHighFitness);
        Entry<SampleIndividual, SampleBehavior> entryLowFitness = new(individual, new SampleBehavior(), 0);

        // Act 
        bool success = archive.TryAdd(0, entryLowFitness);

        // Assert
        Assert.That(success, Is.False);
    }

    [Test]
    public void TryAddingEntry_ThatExistsWithLowerFitness_WillSucceed()
    {
        // Arrange
        Archive<SampleIndividual, SampleBehavior, int> archive = new();
        SampleIndividual individual = new SampleIndividual();
        Entry<SampleIndividual, SampleBehavior> entryLowFitness = new(individual, new SampleBehavior(), 0);
        archive.TryAdd(0, entryLowFitness);
        Entry<SampleIndividual, SampleBehavior> entryHighFitness = new(individual, new SampleBehavior(), 10);

        // Act 
        bool success = archive.TryAdd(0, entryHighFitness);

        // Assert
        Assert.That(success, Is.True);
    }
}