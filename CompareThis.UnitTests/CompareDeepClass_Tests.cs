using System;
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
            Class = new ClassWithOtherClass()
            {
                BaseClass = new BasicClass()
                {
                    DateTimeProperty = new DateTime(2019, 1, 1),
                    IntProperty = 321,
                    StringProperty = "321"
                },
                ManyPropClass = new ManyProperties()
                {
                    Int1 = 321,
                    Int2 = 352565321,
                    Int3 = 4151213,
                    Str1 = "Some String number one ;)",
                    Str2 = "Some string number two :D",
                    Str3 = "mambo number five",
                    Str4 = "uga buga buga u FILTER!"
                },
                someInt = 321,
                someString = "some string : |"
            };
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
