using System;

namespace CompareThis
{
    static class TypeGetters
    {
        public static bool CheckIfTypeIsNullable(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}
