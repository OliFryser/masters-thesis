using BenchmarkDotNet.Attributes;

namespace Benchmarks;

// set build configuration to "release"

public class ExampleClass
{
    [Benchmark]
    public int MyMethod()
    {
        // Do work here... 
        // Return something so the method call is not optimized away.
        return 0;
    }
}