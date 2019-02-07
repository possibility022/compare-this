using System;
using System.Collections;
using System.Collections.Generic;
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
        public Expression ConstantTrue { get; } = Expression.Constant(true);
        public Expression ConstantFalse { get; } = Expression.Constant(false);

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
            else if (
                type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return GetEnumerableExpression(str, value, type);
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

        public Expression GetEnumerableExpression(Expression filter, Expression collection, Type collectionType)
        {
            var genericType = collectionType.GetGenericArguments()[0];
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(genericType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(genericType);

            var value = Expression.Parameter(genericType, "value");

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            var contectOfLoop = GetExpression(genericType, filter, value);

            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label(typeof(bool));

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.IsTrue(moveNextCall),
                        Expression.Block(new[] { value },
                            Expression.Assign(value, Expression.Property(enumeratorVar, "Current")),
                            Expression.IfThen(
                                Expression.IsTrue(contectOfLoop),
                                Expression.Return(breakLabel, ConstantTrue))
                        ),
                        Expression.Return(breakLabel, ConstantFalse)
                    ),
                breakLabel)
            );
            
            return Expression.AndAlso(Expression.NotEqual(collection, ConstantNull), loop);
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
