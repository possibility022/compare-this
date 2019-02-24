using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompareThis.UnitTests
{
    [TestClass]
    public class CompareFactory_TestCollections
    {

        private ClassWithCollection Class { get; set; }
        private Func<ClassWithCollection, string, bool> FilterFunc;

        string _filter = "FILTER!";

        [TestInitialize]
        public void TestInit()
        {
            Class = new ClassWithCollection();
            FilterFunc = CompareFactory.BuildContainsFunc<ClassWithCollection>();
            var FilterExpr = CompareFactory.BuildContainsExpr<ClassWithCollection>(Expression.Parameter(typeof(ClassWithCollection), "someclass"), Expression.Parameter(typeof(string), "filter"));

            var str = FilterExpr.ToString();
        }

        [TestMethod]
        public void ThisMustBeFalse()
        {
            Class.SomeCollection = DataGenerator.GetListOfRandomStrings(100, 10);
            Assert.IsFalse(FilterFunc(Class, _filter));
        }

        [TestMethod]
        public void ThisMustBeTrue()
        {
            Class.SomeCollection = DataGenerator.GetListOfRandomStrings(100, 10);
            Class.SomeCollection.Add(_filter);
            Assert.IsTrue(FilterFunc(Class, _filter));
        }

        [TestMethod]
        public void PropertyIsNull()
        {
            Class.SomeCollection = null;
            Assert.IsFalse(FilterFunc(Class, _filter));
        }

        [TestMethod]
        public void TestArray_Contains_LastIndex()
        {
            Class.ArrayOfInt = DataGenerator.GetArrayOfInt(100);
            Class.ArrayOfInt[Class.ArrayOfInt.Length - 1] = 123321123;
            _filter = Class.ArrayOfInt[Class.ArrayOfInt.Length -1].ToString();

            var results = FilterFunc(Class, _filter);

            Assert.IsTrue(results);
        }

        [TestMethod]
        public void TestArray_Contains_FirstIndex()
        {
            Class.ArrayOfInt = DataGenerator.GetArrayOfInt();
            Class.ArrayOfInt[0] = 321321321;
            _filter = Class.ArrayOfInt[0].ToString();

            var results = FilterFunc(Class, _filter);

            Assert.IsTrue(results);
        }

        [TestMethod]
        public void TestArray_DoesNotContains()
        {
            Class.ArrayOfInt = DataGenerator.GetArrayOfInt(100);

            var results = FilterFunc(Class, _filter);

            Assert.IsFalse(results);
        }

        [TestMethod]
        public void TestArray_ArrayIsNull()
        {
            Class.ArrayOfInt = null;

            var results = FilterFunc(Class, _filter);

            Assert.IsFalse(results);
        }

    }
}
