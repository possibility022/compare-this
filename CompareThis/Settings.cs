using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CompareThis
{
    public class Settings
    {
        public Settings()
        {

        }

        public CompareOptions StringCompareOptions { get; set; } = CompareOptions.IgnoreCase;

        public CompareInfo StringCompareInfo { get; set; } = CultureInfo.CurrentCulture.CompareInfo;

        public string DateTimeToStringFormat { get; set; } = string.Empty;

        public int Deep { get; set; } = 20;

        public IReadOnlyCollection<Type> BlackListProperties { get => _blackListProperties; }
        public IReadOnlyCollection<Type> WhiteListProperties { get => _whiteListProperties; }

        private HashSet<Type> _blackListProperties;
        private HashSet<Type> _whiteListProperties;

        public void AddPropertyToWhiteList(Type type)
        {
            if (BlackListProperties != null)
                throw new InvalidOperationException("Blacklist contains items. Only white list or Black list of properties can be provided at once.");

            if (_whiteListProperties == null)
                _whiteListProperties = new HashSet<Type>();

            type = HandleNullable(type);

            _whiteListProperties.Add(type);
        }

        public void AddPropertyToBlackList(Type type)
        {
            if (WhiteListProperties != null)
                throw new InvalidOperationException("Whitelist contains items. Only white list or Black list of properties can be provided at once.");

            if (_blackListProperties == null)
                _blackListProperties = new HashSet<Type>();

            type = HandleNullable(type);

            _blackListProperties.Add(type);
        }

        private static Type HandleNullable(Type type)
        {
            if (TypeGetters.CheckIfTypeIsNullable(type))
                type = Nullable.GetUnderlyingType(type);
            return type;
        }

        public void ClearLists()
        {
            _whiteListProperties = null;
            _blackListProperties = null;
        }

    }
}
