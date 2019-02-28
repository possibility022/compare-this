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

        Func<ClassWithOtherClass, string, bool> CompareThisFunc = CompareFactory.BuildContainsFunc<ClassWithOtherClass>();

        [TestInitialize]
        public void TestInit()
        {
            Class = DataGenerator.GetFilledUpClassWithOtherClasses();
            Class.ManyPropClass.Str4 = Filter;
        }

        [TestMethod]
        public void ThisMustBeTrue()
        {
            Assert.IsTrue(Class.Filter(Filter));
        }

        [TestMethod]
        public void CompareThisReturnsTrue()
        {
            Assert.IsTrue(CompareThisFunc(Class, Filter));
        }

        [TestMethod]
        public void CompareThisReturnsTrue_CheckNullableClass()
        {
            Assert.IsTrue(CompareThisFunc(Class, Class.ClassWithNullables.NullableInt.ToString()));
        }

        [TestMethod]
        public void AnClassIsNull()
        {
            Class.BaseClass = null;

            Assert.IsTrue(CompareThisFunc(Class, Filter));
        }
    }
}
