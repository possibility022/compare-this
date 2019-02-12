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
        // byte.ToString()
        public MethodInfo ByteToStringMethod { get; } = typeof(byte).GetMethod("ToString", new Type[] { });
        // bool.ToString()
        public MethodInfo BoolToStringMethod { get; } = typeof(bool).GetMethod("ToString", new Type[] { });

        public Expression ConstantNull { get; } = Expression.Constant(null);
        public Expression ConstantTrue { get; } = Expression.Constant(true);
        public Expression ConstantFalse { get; } = Expression.Constant(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stringValue">For example filter.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Expression GetExpression(Type type, Expression stringValue, Expression value)
        {
            if (type == typeof(int))
            {
                return GetIntExpression(stringValue, value);
            }
            else if (type == typeof(string))
            {
                return GetStringExpression(stringValue, value);
            }
            else if (type == typeof(DateTime))
            {
                return GetDateTimeExpression(stringValue, value);
            }
            else if (type == typeof(byte))
            {
                return GetByteExpression(stringValue, value);
            }
            else if (type == typeof(bool))
            {
                return GetBoolExpression(stringValue, value);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var hasValueProperty = type.GetProperty("HasValue");
                var valueProperty = type.GetProperty("Value");

                var hasValuePropertyExpression = Expression.Property(value, hasValueProperty);
                var valuePropertyExpressionExpression = Expression.Property(value, valueProperty);

                return Expression.AndAlso(
                    hasValuePropertyExpression, // LEFT
                    GetExpression(Nullable.GetUnderlyingType(type), stringValue, valuePropertyExpressionExpression)); // RIGHT
            }
            else if (
                type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return GetEnumerableExpression(stringValue, value, type);
            }
            else if (type.IsClass)
            {
                var method = typeof(CompareFactory)
                    .GetMethod("BuildContainsExpr", new Type[] { typeof(Expression), typeof(Expression) });

                var genericMethod = method.MakeGenericMethod(type);
                return (Expression)genericMethod.Invoke(null, new object[] { value, stringValue });
            }
            else
            {
                throw new NotSupportedException($"Type of {type.FullName} is not supported.");
            }
        }

        private Expression GetBoolExpression(Expression filter, Expression @bool)
        {
            var callBoolToString = Expression.Call(@bool, BoolToStringMethod);
            return Expression.Call(callBoolToString, StringContainsMethod, filter);
        }

        private Expression GetByteExpression(Expression filter, Expression @byte)
        {
            var callByteToString = Expression.Call(@byte, ByteToStringMethod);
            return Expression.Call(callByteToString, StringContainsMethod, filter);
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

        public Expression GetDateTimeExpression(Expression filter, Expression dateTime)
        {
            var callDateTimeToString = Expression.Call(dateTime, DateTimeToString);
            return Expression.Call(callDateTimeToString, StringContainsMethod, filter);
        }
    }
}
