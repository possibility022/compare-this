using System;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompareThis.UnitTests
{
    [TestClass]
    public class CompareDeepClass_Tests
    {
        private ClassWithOtherClass Class { get; set; }

        const string Filter = "FILTER!";

        [TestInitialize]
        public void TestInit()
        {
            Class = DataGenerator.GetFilledUpClassWithOtherClasses();
        }
               
        [TestMethod]
        public void ThisMustBeTrue()
        {
            Assert.IsTrue(Class.Filter(Filter));
        }

        [TestMethod]
        public void CompareThisReturnsTrue()
        {
            var compareFunc = CompareFactory.BuildContainsFunc<ClassWithOtherClass>();

            Assert.IsTrue(compareFunc(Class, Filter));

        }
    }
}
