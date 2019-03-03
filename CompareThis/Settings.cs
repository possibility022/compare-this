using System.Globalization;

namespace CompareThis
{
    public class Settings
    {
        public CompareOptions StringCompareOptions { get; set; } = CompareOptions.IgnoreCase;

        public CompareInfo StringCompareInfo { get; set; } = CultureInfo.CurrentCulture.CompareInfo;

        public string DateTimeToStringFormat { get; set; } = string.Empty;

        public int Deep { get; set; } = 20;
    }
}
