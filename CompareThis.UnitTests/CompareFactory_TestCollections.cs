using System;
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

        const string Filter = "FILTER!";

        [TestInitialize]
        public void TestInit()
        {
            Class = DataGenerator.GetClassWithCollection();
            FilterFunc = CompareFactory.BuildContainsFunc<ClassWithCollection>();
        }

        [TestMethod]
        public void ThisMustBeFalse()
        {
            Assert.IsFalse(FilterFunc(Class, Filter));
        }

        [TestMethod]
        public void ThisMustBeTrue()
        {
            Class.SomeCollection.Add(Filter);
            Assert.IsTrue(FilterFunc(Class, Filter));
        }

        [TestMethod]
        public void PropertyIsNull()
        {
            Class.SomeCollection = null;
            Assert.IsFalse(FilterFunc(Class, Filter));
        }

    }
}
