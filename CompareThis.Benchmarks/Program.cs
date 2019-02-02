using BenchmarkDotNet.Running;
using CompareThis.Benchmarks.Benchmarks;
using System;

namespace CompareThis.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var results = BenchmarkRunner.Run<ClassWithOtherClassCompare>();
            Console.ReadLine();

            results = BenchmarkRunner.Run<OnePropBenchmark>();
            Console.ReadLine();

            results = BenchmarkRunner.Run<BasicClassBenchmark>();
            Console.ReadLine();

            results = BenchmarkRunner.Run<ManyPropertiesBenchmark>();
            Console.ReadLine();
        }
    }
}
