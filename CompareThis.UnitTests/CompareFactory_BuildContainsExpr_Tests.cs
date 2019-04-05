using System;
using System.Collections.Generic;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompareThis.UnitTests
{
    [TestClass]
    public class CompareFactory_BuildContainsExpr_Tests
    {

        private bool CompareTo(BasicClass basicClass, string filter)
        {
            return (filter != null)
                && ((basicClass.StringProperty != null && basicClass.StringProperty.Contains(filter))
                || (basicClass.IntProperty.ToString().Contains(filter))
                || (basicClass.DateTimeProperty.HasValue && basicClass.DateTimeProperty.Value.ToString().Contains(filter))
                );
        }

        IList<BasicClass> ListToCheck = DataGenerator.GetBasicClass(200);
        Func<BasicClass, string, bool> FunctionToTest;

        const string Filter = "!FILTER!";

        [TestInitialize]
        public void TestInitialize()
        {
            FunctionToTest = CompareFactory.BuildContainsFunc<BasicClass>();
            ListToCheck[ListToCheck.Count - 1].StringProperty = Filter;
        }
        
        [TestMethod]
        [Description("Brute Force testing.")]
        public void ResultsMustBeEqual()
        {
            foreach(var obj in ListToCheck)
            {
                var resultOfFunc = FunctionToTest(obj, Filter);
                var resultsOfCustomFunc = CompareTo(obj, Filter);

                Assert.AreEqual(resultsOfCustomFunc, resultOfFunc);
            }
        }

        [TestMethod]
        public void ResultMustBeTrue()
        {
            var result = FunctionToTest(ListToCheck[ListToCheck.Count - 1], Filter);

            Assert.IsTrue(result);
        }
    }
}
