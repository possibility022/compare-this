using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        public Expression constantNull { get; } = Expression.Constant(null);

        public Expression GetExpression(Type type, Expression parameterFilter, Expression propExpressions)
        {
            if (type == typeof(int))
            {
                return GetIntExpression(parameterFilter, propExpressions);
            }
            else if (type == typeof(string))
            {
                return GetStringExpression(parameterFilter, propExpressions);
            }
            else if (type == typeof(DateTime?))
            {
                return GetStringExpression(parameterFilter, propExpressions);
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

        public Expression GetStringExpression(Expression parameterFilter, Expression propExpressions)
        {
            var propIsNotNull = Expression.NotEqual(propExpressions, constantNull);
            var callContains = Expression.Call(propExpressions, stringContainsMethod, parameterFilter);
            return Expression.AndAlso(propIsNotNull, callContains);
        }

        public Expression GetNullableDateTimeExpression(Expression parameterFilter, Expression propExpressions)
        {
            var nullDateTimeProperties = typeof(DateTime?).GetProperties();

            var hasValueExpression = Expression.Property(
                propExpressions, nullDateTimeProperties.Where(p => p.Name == "HasValue").First());

            var valueExpression = Expression.Property(
                propExpressions, nullDateTimeProperties.Where(p => p.Name == "Value").First());
            var callDateTimeToString = Expression.Call(valueExpression, dateTimeToString);
            var contains = Expression.Call(callDateTimeToString, stringContainsMethod, parameterFilter);
            return Expression.AndAlso(hasValueExpression, contains);
        }
    }
}
