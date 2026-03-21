using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace DangFugBixs.Generators.Benchmarks
{
    public class GeneratorBenchmarks
    {
        [Benchmark]
        public void SimpleLoop()
        {
            int sum = 0;
            for (int i = 0; i < 1000; i++)
                sum += i;
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<GeneratorBenchmarks>();
        }
    }
}
