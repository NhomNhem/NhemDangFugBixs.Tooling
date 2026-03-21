using BenchmarkDotNet.Attributes;

namespace DangFugBixs.Generators.Benchmarks
{
    public class SampleLoopBenchmarks
    {
        [Benchmark]
        public void SimpleLoop()
        {
            int sum = 0;
            for (int i = 0; i < 1000; i++)
                sum += i;
        }
    }
}
