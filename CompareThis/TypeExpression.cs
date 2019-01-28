using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CompareThis
{
    public class TypeExpression
    {
        // Methods to call.
        // string.Contains(param)
        public MethodInfo stringContainsMethod { get; } = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        // int.ToString()
        public MethodInfo intToStringMethod { get; } = typeof(int).GetMethod("ToString", new Type[] { });
        // DateTimeToString()
        public MethodInfo dateTimeToString { get; } = typeof(DateTime).GetMethod("ToString", new Type[] { });

        public Expression GetExpression(Type type, Expression parameterFilter, Expression propExpressions)
        {
            if (type == typeof(int))
            {
                return GetIntExpression(parameterFilter, propExpressions);
            }
            else
            {
                throw new NotSupportedException($"Type of {type.FullName} is not supported.");
            }
        }

        public Expression GetIntExpression(Expression parameterFilter, Expression propExpressions)
        {
            var callIntToString = Expression.Call(propExpressions, intToStringMethod);
            return Expression.Call(callIntToString, stringContainsMethod, parameterFilter);
        }
    }
}
