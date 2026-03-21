using BenchmarkDotNet.Running;

namespace DangFugBixs.Generators.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<SampleLoopBenchmarks>();
        }
    }
}
