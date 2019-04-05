using CompareThis.Exceptions;
using CompareThis.Utilities.DataGenerator;
using CompareThis.Utilities.ExampleClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CompareThis.UnitTests
{
    [TestClass]
    public class SettingsTests
    {

        Func<BasicClass, string, bool> FilterFunc_BasicFunc;
        Func<ClassWithOtherClass, string, bool> FilterFunc_ClassWithOtherClasses;
        BasicClass basicClass = new BasicClass();
        string filter;
        const string dateTimeFormat = "MM/dd/yyyy hh:mm tt";

        [TestInitialize]
        public void TestInit()
        {
            basicClass = new BasicClass()
            {
                DateTimeProperty = new DateTime(2001, 12, 2, 8, 12, 23, 0)
            };
            filter = "12.02.2001 08:12 AM";

            if (filter != basicClass.DateTimeProperty.Value.ToString(dateTimeFormat))
                throw new Exception("woops");

        }

        [TestMethod]
        public void TestDateTimeFormat()
        {
            var settings = new Settings()
            {
                DateTimeToStringFormat = dateTimeFormat
            };

            FilterFunc_BasicFunc = CompareFactory.BuildContainsFunc<BasicClass>(settings);

            Assert.IsTrue(FilterFunc_BasicFunc.Invoke(basicClass, filter));
        }

        [TestMethod]
        public void TestDeepOption_False()
        {
            var settings = new Settings()
            {
                Deep = 1
            };

            var classWithOtherClass = new ClassWithOtherClass()
            {
                BaseClass = new BasicClass() { StringProperty = "FILTER!" }
            };

            FilterFunc_ClassWithOtherClasses = CompareFactory.BuildContainsFunc<ClassWithOtherClass>(settings);

            Assert.IsFalse(FilterFunc_ClassWithOtherClasses(classWithOtherClass, "FILTER!"));
        }

        [TestMethod]
        public void TestDeepOption_True()
        {
            var classWithOtherClass = new ClassWithOtherClass()
            {
                BaseClass = new BasicClass() { StringProperty = "FILTER!" }
            };

            FilterFunc_ClassWithOtherClasses = CompareFactory.BuildContainsFunc<ClassWithOtherClass>();

            Assert.IsTrue(FilterFunc_ClassWithOtherClasses(classWithOtherClass, "FILTER!"));
        }

        [TestMethod]
        public void TestDeepOption_DeepLevel5_True()
        {
            var classLvl0 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl1 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl2 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl3 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl4 = DataGenerator.GetFilledUpClassWithOtherClasses();

            classLvl0.ClassWithOtherClassProp = classLvl1;
            classLvl1.ClassWithOtherClassProp = classLvl2;
            classLvl2.ClassWithOtherClassProp = classLvl3;
            classLvl3.ClassWithOtherClassProp = classLvl4;
            classLvl4.BaseClass.StringProperty = "Filter!!!!";

            FilterFunc_ClassWithOtherClasses = CompareFactory.BuildContainsFunc<ClassWithOtherClass>();

            Assert.IsTrue(FilterFunc_ClassWithOtherClasses(classLvl0, "Filter!!!!"));
        }

        [TestMethod]
        public void TestDeepOption_DeepLevel5_False()
        {
            var settings = new Settings()
            {
                Deep = 4
            };

            var classLvl0 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl1 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl2 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl3 = DataGenerator.GetFilledUpClassWithOtherClasses();
            var classLvl4 = DataGenerator.GetFilledUpClassWithOtherClasses();

            classLvl0.ClassWithOtherClassProp = classLvl1;
            classLvl1.ClassWithOtherClassProp = classLvl2;
            classLvl2.ClassWithOtherClassProp = classLvl3;
            classLvl3.ClassWithOtherClassProp = classLvl4;
            classLvl4.BaseClass.StringProperty = "Filter!!!!";

            FilterFunc_ClassWithOtherClasses = CompareFactory.BuildContainsFunc<ClassWithOtherClass>(settings);

            Assert.IsFalse(FilterFunc_ClassWithOtherClasses(classLvl0, "Filter!!!!"));
        }

        [TestMethod]
        public void TestWhiteList_TypeIsOnTheList()
        {
            var set = new Settings();
            set.AddPropertyToWhiteList(typeof(DateTime?));
            set.DateTimeToStringFormat = dateTimeFormat;

            var func = CompareFactory.BuildContainsFunc<BasicClass>(set);

            Assert.IsTrue(func(basicClass, filter));
        }

        [TestMethod]
        public void TestWhiteList_TypeIsNOTonTheList()
        {
            var set = new Settings();
            set.AddPropertyToWhiteList(typeof(string));

            var func = CompareFactory.BuildContainsFunc<BasicClass>(set);

            Assert.IsFalse(func(basicClass, filter));
        }

        [TestMethod]
        public void TestBlackList_TypeIsOnTheList()
        {
            var set = new Settings();
            set.DateTimeToStringFormat = dateTimeFormat;
            set.AddPropertyToBlackList(typeof(DateTime));

            var func = CompareFactory.BuildContainsFunc<BasicClass>(set);

            Assert.IsFalse(func(basicClass, filter));
        }

        [TestMethod]
        public void TestBlackList_TypeIsNOTonTheList()
        {
            var set = new Settings();
            set.DateTimeToStringFormat = dateTimeFormat;
            set.AddPropertyToBlackList(typeof(string));

            var func = CompareFactory.BuildContainsFunc<BasicClass>(set);

            Assert.IsTrue(func(basicClass, filter));
        }
    }
}
