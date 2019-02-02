using System;
using BenchmarkDotNet.Attributes;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;

namespace CompareThis.Benchmarks.Benchmarks
{
    [RankColumn]
    public class ClassWithOtherClassCompare
    {
        ClassWithOtherClass Class = DataGenerator.GetFilledUpClassWithOtherClasses();

        Func<ClassWithOtherClass, string, bool> CompareThisFunc = CompareFactory.BuildContainsFunc<ClassWithOtherClass>();

        string Filter = "SOMESTRING";

        [Benchmark]
        public bool TestCompareThis()
        {
            return CompareThisFunc(Class, Filter);
        }

        [Benchmark]
        public bool UseBuildInFunction()
        {
            return Class.Filter(Filter);
        }
    }
}
