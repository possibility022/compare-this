using System;
using System.Collections.Generic;

namespace CompareThis
{
    public static class SettingsExtensions
    {
        public static void SetStandardWhiteList(this Settings settings)
        {
            settings.AddPropertyToWhiteList(typeof(int));
            settings.AddPropertyToWhiteList(typeof(string));
            settings.AddPropertyToWhiteList(typeof(byte));
            settings.AddPropertyToWhiteList(typeof(DateTime));
            settings.AddPropertyToWhiteList(typeof(int[]));
            settings.AddPropertyToWhiteList(typeof(string[]));
            settings.AddPropertyToWhiteList(typeof(IEnumerable<string>));
            settings.AddPropertyToWhiteList(typeof(IEnumerable<int>));
        }

    }
}
