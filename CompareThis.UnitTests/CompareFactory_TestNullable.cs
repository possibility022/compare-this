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
        ClassWithNullables nullableClass;
        Func<ClassWithNullables, string, bool> CompareThisFunc;

        [TestInitialize]
        public void TestInit()
        {
            nullableClass = new ClassWithNullables();

            CompareThisFunc = CompareFactory.BuildContainsFunc<ClassWithNullables>();
        }

        [TestMethod]
        public void TestNullable_BoolContains()
        {
            // Arrange
            nullableClass.NullableBool = false;

            // Act
            var results = CompareThisFunc.Invoke(nullableClass, false.ToString());

            //Assert
            Assert.IsTrue(results);
        }

        [TestMethod]
        public void TestNullable_DateTimeContains()
        {
            // Arrange
            var dateTime = new DateTime(2000, 10, 10);
            nullableClass.NullableDateTime = dateTime;

            // Act
            var results = CompareThisFunc.Invoke(nullableClass, dateTime.ToString());

            //Assert
            Assert.IsTrue(results);
        }

        [TestMethod]
        public void TestNullable_ByteContains()
        {
            // Arrange
            nullableClass.NullableByte = 0x1;

            // Act
            var results = CompareThisFunc.Invoke(nullableClass, 0x1.ToString());

            //Assert
            Assert.IsTrue(results);
        }

        [TestMethod]
        public void TestNullable_IntContains()
        {
            // Arrange
            nullableClass.NullableInt = 111111;

            // Act
            var results = CompareThisFunc.Invoke(nullableClass, 111111.ToString());

            //Assert
            Assert.IsTrue(results);
        }


    }
}
