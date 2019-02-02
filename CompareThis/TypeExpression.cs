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
        public MethodInfo StringContainsMethod { get; } = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        // int.ToString()
        public MethodInfo IntToStringMethod { get; } = typeof(int).GetMethod("ToString", new Type[] { });
        // DateTimeToString()
        public MethodInfo DateTimeToString { get; } = typeof(DateTime).GetMethod("ToString", new Type[] { });

        public Expression ConstantNull { get; } = Expression.Constant(null);

        public Expression GetExpression(Type type, Expression str, Expression value)
        {
            if (type == typeof(int))
            {
                return GetIntExpression(str, value);
            }
            else if (type == typeof(string))
            {
                return GetStringExpression(str, value);
            }
            else if (type == typeof(DateTime?))
            {
                return GetNullableDateTimeExpression(str, value);
            }
            else if (type.IsClass)
            {
                var method = typeof(CompareFactory)
                    .GetMethod("BuildContainsExpr", new Type[] { typeof(Expression), typeof(Expression) });

                var genericMethod = method.MakeGenericMethod(type);
                return (Expression)genericMethod.Invoke(null, new object[] { value, str });
            }
            else
            {
                throw new NotSupportedException($"Type of {type.FullName} is not supported.");
            }
        }

        public Expression GetIntExpression(Expression filter, Expression integer)
        {
            var callIntToString = Expression.Call(integer, IntToStringMethod);
            return Expression.Call(callIntToString, StringContainsMethod, filter);
        }

        public Expression GetStringExpression(Expression filter, Expression str)
        {
            var propIsNotNull = Expression.NotEqual(str, ConstantNull);
            var callContains = Expression.Call(str, StringContainsMethod, filter);
            return Expression.AndAlso(propIsNotNull, callContains);
        }

        public Expression GetNullableDateTimeExpression(Expression filter, Expression dateTime)
        {
            var nullDateTimeProperties = typeof(DateTime?).GetProperties();

            var hasValueExpression = Expression.Property(
                dateTime, nullDateTimeProperties.Where(p => p.Name == "HasValue").First());

            var valueExpression = Expression.Property(
                dateTime, nullDateTimeProperties.Where(p => p.Name == "Value").First());
            var callDateTimeToString = Expression.Call(valueExpression, DateTimeToString);
            var contains = Expression.Call(callDateTimeToString, StringContainsMethod, filter);
            return Expression.AndAlso(hasValueExpression, contains);
        }
    }
}
