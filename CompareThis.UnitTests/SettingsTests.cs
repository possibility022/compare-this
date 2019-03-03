using CompareThis.Utilities.ExampleClass;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CompareThis.UnitTests
{
    [TestClass]
    public class SettingsTests
    {

        Func<BasicClass,string, bool> FilterFunc;
        BasicClass basicClass = new BasicClass();
        string filter;

        [TestInitialize]
        public void TestInit()
        {
            basicClass = new BasicClass()
            {
                DateTimeProperty = new DateTime(2001, 12, 2, 8, 12, 23, 0)
            };
            filter = "12.02.2001 08:12 AM";
        }

        [TestMethod]
        public void TestDateTimeFormat()
        {
            var settings = new Settings()
            {
                DateTimeToStringFormat = "MM/dd/yyyy hh:mm tt"
            };

            FilterFunc = CompareFactory.BuildContainsFunc<BasicClass>(settings);

            Assert.IsTrue(FilterFunc.Invoke(basicClass, filter));

        }
    }
}
