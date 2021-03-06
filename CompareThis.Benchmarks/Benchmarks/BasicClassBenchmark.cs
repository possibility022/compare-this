﻿using BenchmarkDotNet.Attributes;
using CompareThis.Utilities.ExampleClass;
using System;
using System.Reflection;

namespace CompareThis.Benchmarks.Benchmarks
{

    [RPlotExporter, RankColumn]
    public class BasicClassBenchmark
    {
        private string Filter = "_SOMESTRING_";
        private BasicClass BasicClassForTest = new BasicClass()
        {
            DateTimeProperty = DateTime.Now,
            IntProperty = 321,
            StringProperty = "SOME STRING"
        };

        private static PropertyInfo[] Properties = typeof(BasicClass).GetProperties();

        private Func<BasicClass, string, bool> CompiledFunc = CompareFactory.BuildContainsFunc<BasicClass>();

        private bool UsingListOfProperties(BasicClass BasicClass, string filter)
        {
            for (int i = 0; i < Properties.Length; i++)
            {
                var val = Properties[i].GetValue(BasicClass);
                if (val != null && val.ToString() == filter)
                    return true;
            }
            return false;
        }

        private bool ExternalCompare(BasicClass basicClass, string filter)
        {
            return (filter != null)
                && ((basicClass.StringProperty != null && basicClass.StringProperty.Contains(filter))
                || (basicClass.IntProperty.ToString().Contains(filter))
                || (basicClass.DateTimeProperty.HasValue && basicClass.DateTimeProperty.Value.ToString().Contains(filter))
                );
        }

        [Benchmark]
        public bool BasingOnProperties()
        {
            return UsingListOfProperties(BasicClassForTest, Filter);
        }

        [Benchmark]
        public bool BasinOnFuncInsideOfClass()
        {
            return BasicClassForTest.Filter(Filter);
        }

        [Benchmark]
        public bool UsingCompiledExpressonTreeFunc()
        {
            return CompiledFunc(BasicClassForTest, Filter);
        }

        [Benchmark]
        public bool UsingExternalFunction()
        {
            return ExternalCompare(BasicClassForTest, Filter);
        }

    }
}
