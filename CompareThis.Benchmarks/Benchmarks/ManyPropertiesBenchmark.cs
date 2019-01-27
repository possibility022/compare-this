using BenchmarkDotNet.Attributes;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;
using System;
using System.Reflection;

namespace CompareThis.Benchmarks.Benchmarks
{
    public class ManyPropertiesBenchmark
    {

        private string Filter = "123";

        public bool ExternalCompare(ManyProperties manyProperties, string filter)
        {
            return ((filter != null)
                && (
                manyProperties.Int1.ToString().Contains(filter)
                || manyProperties.Int2.ToString().Contains(filter)
                || manyProperties.Int3.ToString().Contains(filter)
                || (manyProperties.Str1 != null) && (manyProperties.Str1.Contains(filter))
                || (manyProperties.Str2 != null) && (manyProperties.Str2.Contains(filter))
                || (manyProperties.Str3 != null) && (manyProperties.Str3.Contains(filter))
                || (manyProperties.Str4 != null) && (manyProperties.Str4.Contains(filter))
                )
                );
        }

        private ManyProperties ManyPropertiesForTest = new ManyProperties()
        {
            Int1 = 10,
            Int2 = 321,
            Int3 = 321,
            Str1 = "ABC",
            Str2 = null,
            Str3 = "SOME VERY LONG TEXT TO CHECK, SOME VERY LONG TEXT TO CHECK, SOME VERY LONG TEXT TO CHECK",
            Str4 = "123"
        };

        private static PropertyInfo[] ManyProperties = typeof(ManyProperties).GetProperties();

        private Func<ManyProperties, string, bool> CompiledFunc_ManyProp = CompareFactory.BuildContainsFunc<ManyProperties>();

        private bool UsingListOfProperties_Many(ManyProperties BasicClass, string filter)
        {
            for (int i = 0; i < ManyProperties.Length; i++)
            {
                var val = ManyProperties[i].GetValue(BasicClass);
                if (val != null && val.ToString() == filter)
                    return true;
            }
            return false;
        }

        [Benchmark]
        public bool BasingOnProperties_ManyProperties()
        {
            return UsingListOfProperties_Many(ManyPropertiesForTest, Filter);
        }

        [Benchmark]
        public bool BasinOnFuncInsideOfClass_ManyProperties()
        {
            return ManyPropertiesForTest.Filter(Filter);
        }

        [Benchmark]
        public bool UsingCompiledExpressonTreeFunc_ManyProperties()
        {
            return CompiledFunc_ManyProp(ManyPropertiesForTest, Filter);
        }

        [Benchmark]
        public bool UsingExternalFunction_ManyProperties()
        {
            return ExternalCompare(ManyPropertiesForTest, Filter);
        }
    }
}
