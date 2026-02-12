namespace LibraryTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.That(Library.Class1.PrintingString, Is.EqualTo("Hallelujah"));
    }
}