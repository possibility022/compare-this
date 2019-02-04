using BenchmarkDotNet.Running;
using CompareThis.Benchmarks.Benchmarks;
using System;

namespace CompareThis.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CollectionBenchmark>();
            Console.ReadLine();

            BenchmarkRunner.Run<ClassWithOtherClassCompare>();
            Console.ReadLine();

            BenchmarkRunner.Run<OnePropBenchmark>();
            Console.ReadLine();

            BenchmarkRunner.Run<BasicClassBenchmark>();
            Console.ReadLine();

            BenchmarkRunner.Run<ManyPropertiesBenchmark>();
            Console.ReadLine();
        }
    }
}
