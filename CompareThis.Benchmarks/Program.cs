using BenchmarkDotNet.Running;
using CompareThis.Benchmarks.Benchmarks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareThis.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var results = BenchmarkRunner.Run<BasicClassBenchmark>();
            Console.ReadLine();

            results = BenchmarkRunner.Run<ManyPropertiesBenchmark>();
            Console.ReadLine();
        }
    }
}
