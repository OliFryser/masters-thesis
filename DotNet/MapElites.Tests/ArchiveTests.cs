using MapElites.Models;

namespace MapElites.Tests;

public class Tests
{
    [Test]
    public void SamplingArchive_WithOneEntry_ReturnsTheEntry()
    {
        // Arrange
        Archive<int, SampleEntry, SampleIndividual, SampleBehavior> archive = new();
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entry = new(individual, new SampleBehavior(), 0);
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
        Archive<int, SampleEntry, SampleIndividual, SampleBehavior> archive = new();

        // Act 
        Func<SampleIndividual> sample = archive.SampleRandom;

        // Assert
        Assert.That(sample, Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TryAddingEntry_ToEmptyArchive_WillSucceed()
    {
        // Arrange
        Archive<int, SampleEntry, SampleIndividual, SampleBehavior> archive = new();
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entry = new(individual, new SampleBehavior(), 0);

        // Act 
        bool success = archive.TryAdd(0, entry);

        // Assert
        Assert.That(success, Is.True);
    }

    [Test]
    public void TryAddingEntry_ThatAlreadyExistsWithBetterFitness_WillFail()
    {
        // Arrange
        Archive<int, SampleEntry, SampleIndividual, SampleBehavior> archive = new();
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entryHighFitness = new(individual, new SampleBehavior(), 10);
        archive.TryAdd(0, entryHighFitness);
        SampleEntry entryLowFitness = new(individual, new SampleBehavior(), 0);

        // Act 
        bool success = archive.TryAdd(0, entryLowFitness);

        // Assert
        Assert.That(success, Is.False);
    }

    [Test]
    public void TryAddingEntry_ThatExistsWithLowerFitness_WillSucceed()
    {
        // Arrange
        Archive<int, SampleEntry, SampleIndividual, SampleBehavior> archive = new();
        SampleIndividual individual = new SampleIndividual();
        SampleEntry entryLowFitness = new(individual, new SampleBehavior(), 0);
        archive.TryAdd(0, entryLowFitness);
        SampleEntry entryHighFitness = new(individual, new SampleBehavior(), 10);

        // Act 
        bool success = archive.TryAdd(0, entryHighFitness);

        // Assert
        Assert.That(success, Is.True);
    }
}