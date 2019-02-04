using BenchmarkDotNet.Attributes;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareThis.Benchmarks.Benchmarks
{
    [RankColumn]
    public class CollectionBenchmark
    {
        const string Filter = "Filter";

        ClassWithCollection classWithCollection = DataGenerator.GetClassWithCollection();

        Func<ClassWithCollection, string, bool> CompareThisFunc = CompareFactory.BuildContainsFunc<ClassWithCollection>();

        [Benchmark]
        public bool BuiltInFunc()
        {
            return classWithCollection.Filter(Filter);
        }

        [Benchmark]
        public bool CompareThis()
        {
            return CompareThisFunc(classWithCollection, Filter);
        }
    }
}
