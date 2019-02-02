using BenchmarkDotNet.Attributes;
using CompareThis.Utilities.ExampleClass;
using System;

namespace CompareThis.Benchmarks.Benchmarks
{
    public class OnePropBenchmark
    {
        OnePropClass onePropClass = new OnePropClass()
        {
            Str = "SomeStringHere123"
        };

        Func<OnePropClass, string, bool> CompareThisFunc = CompareFactory.BuildContainsFunc<OnePropClass>();

        string Filter = "123";

        [Benchmark]
        public bool TestCompareThis()
        {
            return CompareThisFunc(onePropClass, Filter);
        }


        [Benchmark]
        public bool UseBuildInFunction()
        {
            return onePropClass.Filter(Filter);
        }
    }
}
