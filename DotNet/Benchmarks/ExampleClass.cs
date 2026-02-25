using BenchmarkDotNet.Attributes;

namespace Benchmarks;

// set build configuration to "release"

public class ExampleClass
{
    [Benchmark]
    public void MyMethod()
    {
        // Do work here... 
    }
}