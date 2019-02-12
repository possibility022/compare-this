using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using CompareThis.Utilities.ExampleClass;

namespace CompareThis.UnitTests
{
    /// <summary>
    /// Summary description for CompareFactory_TestNullable
    /// </summary>
    [TestClass]
    public class CompareFactory_TestNullable
    {
        private const string PropertyName = "NullableDateTime";

        [TestMethod]
        public void TestCustomMethod_ReturnTrue()
        {
            var testClass = new ClassWithNullables() { NullableDateTime = new DateTime(2000, 10, 10) };
            var prop = testClass.GetType().GetProperty(PropertyName);

            Assert.IsTrue(Test(prop, testClass));
        }

        [TestMethod]
        public void TestCustomMethod_ReturnFalse()
        {
            var testClass = new ClassWithNullables() { };
            var prop = testClass.GetType().GetProperty(PropertyName);

            Assert.IsFalse(Test(prop, testClass));
        }

        // The idea is to "translate" this to expression.
        public bool Test(PropertyInfo prop, object o)
        {
            if (!prop.PropertyType.IsGenericType || prop.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                return false;


            var val = prop.GetValue(o);

            return val != null;
            // Expression
        }



    }
}
