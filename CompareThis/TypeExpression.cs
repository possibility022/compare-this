using CompareThis.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CompareThis
{
    public class TypeExpression
    {
        public TypeExpression(Settings settings)
        {
            if (settings == null)
                _settings = new Settings();
            else
                _settings = settings;

            ConstantIgnoreCase = Expression.Constant(_settings.StringCompareOptions);
            ConstantCompareInfo = Expression.Constant(_settings.StringCompareInfo);
            ConstantDateTimeToStringFormat = Expression.Constant(_settings.DateTimeToStringFormat);
        }

        private Settings _settings;


        // Methods to call.
        // string.Contains(param)
        public MethodInfo StringContainsMethod { get; } = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        public MethodInfo StringContainsIngnoreCase { get; } = typeof(CompareInfo).GetMethod("IndexOf", new Type[] { typeof(string), typeof(string), typeof(CompareOptions) });

        // int.ToString()
        public MethodInfo IntToStringMethod { get; } = typeof(int).GetMethod("ToString", new Type[] { });
        // DateTimeToString()
        public MethodInfo DateTimeToString { get; } = typeof(DateTime).GetMethod("ToString", new Type[] { typeof(string) });
        // byte.ToString()
        public MethodInfo ByteToStringMethod { get; } = typeof(byte).GetMethod("ToString", new Type[] { });
        // bool.ToString()
        public MethodInfo BoolToStringMethod { get; } = typeof(bool).GetMethod("ToString", new Type[] { });
        // Console.WriteLine()
        public MethodInfo ConsoleWriteLine_Int { get; } = typeof(DebugHelper).GetMethod("WriteLine", new Type[] { typeof(int) });
        public MethodInfo ConsoleWriteLine_String { get; } = typeof(DebugHelper).GetMethod("WriteLine", new Type[] { typeof(string) });
        public MethodInfo ConsoleWriteLine_Bool { get; } = typeof(DebugHelper).GetMethod("WriteLine", new Type[] { typeof(bool) });


        public Expression ConstantZero { get; } = Expression.Constant(0);
        public Expression ConstantOne { get; } = Expression.Constant(1);
        public Expression ConstantNull { get; } = Expression.Constant(null);
        public Expression ConstantTrue { get; } = Expression.Constant(true);
        public Expression ConstantFalse { get; } = Expression.Constant(false);
        public Expression ConstantIgnoreCase { get; }
        public Expression ConstantCompareInfo { get; }
        public Expression ConstantDateTimeToStringFormat { get; }


        public Expression WriteLineWrapper_Debugger(Expression expression, Type type, string propertyName)
        {
#if DEBUG
            var con = Expression.Constant($"CompareThis: {propertyName} - {type.FullName}", typeof(string));
            var returnTarget = Expression.Label(typeof(bool));
            var returnExpression = Expression.Return(returnTarget, expression, typeof(bool));
            var returnLabel = Expression.Label(returnTarget, ConstantFalse);

            var blockExpr =
                Expression.Block(
                    Expression.Call(Expression.New(typeof(DebugHelper)), ConsoleWriteLine_String, con),
                    returnExpression,
                    returnLabel
                );

            return blockExpr;
            return expression;
#else
            return expression;
#endif
        }

        public Expression WriteLineWrapper_Debugger(Expression stringValue, Expression expression)
        {
#if DEBUG
            var returnTarget = Expression.Label(typeof(bool));
            var returnExpression = Expression.Return(returnTarget, expression, typeof(bool));
            var returnLabel = Expression.Label(returnTarget, ConstantFalse);

            var blockExpr =
                Expression.Block(
                    Expression.Call(Expression.New(typeof(DebugHelper)), ConsoleWriteLine_String, stringValue),
                    returnExpression,
                    returnLabel
                );

            return blockExpr;
            return expression;
#else
            return expression;
#endif
        }

        private bool CanGenerateForThisProperty(Type type)
        {

            if (type != typeof(string) && // Well, for string we have a special method.
                (type.BaseType != null && type.IsArray == false) && // It it is an array then we have to check of which type is this array.
                type
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                type = type.GetGenericArguments()[0];
            }
            else if (type.IsArray)
            {
                new DebugHelper().WriteLine(type.GetElementType().ToString());
            }

            if (TypeGetters.CheckIfTypeIsNullable(type))
                type = Nullable.GetUnderlyingType(type);

            if (_settings.BlackListProperties != null)
            {
                if (_settings.BlackListProperties.Contains(type))
                    return false;
                else
                    return true;
            }
            else
            if (_settings.WhiteListProperties != null)
            {
                if (_settings.WhiteListProperties.Contains(type))
                    return true;
                else
                    return false;
            }

            return true;
        }

        private Expression StringContains(Expression searchIn, Expression value)
        {
            var contains = Expression.Call(ConstantCompareInfo, StringContainsIngnoreCase, searchIn, value, ConstantIgnoreCase);
            return Expression.GreaterThanOrEqual(contains, ConstantZero);
        }


        public Expression GetExpression(Type type, Expression stringValue, Expression value, int deep)
        => GetExpression(type, stringValue, value, deep, false);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stringValue">For example filter.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Expression GetExpression(Type type, Expression stringValue, Expression value, int deep, bool ignoreTypeCheck)
        {

            if (!ignoreTypeCheck && CanGenerateForThisProperty(type) == false)
                return null;

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
            else if (type.IsArray)
            {
                return GetArrayExpression(type, stringValue, value, ref deep);
            }
            else if (type == typeof(bool))
            {
                return GetBoolExpression(stringValue, value);
            }
            else if (TypeGetters.CheckIfTypeIsNullable(type))
            {
                return GetNullableExpression(type, stringValue, value, ref deep);
            }
            else if (
                type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return GetEnumerableExpression(stringValue, value, type, ref deep);
            }
            else if (type.IsClass)
            {
                if (deep + 1 > _settings.Deep)
                    return null;

                var method = typeof(CompareFactory)
                    .GetMethod("BuildContainsExpr", new Type[] { typeof(Expression), typeof(Expression), typeof(TypeExpression), typeof(int) });

                var genericMethod = method.MakeGenericMethod(type);

                return Expression.AndAlso(Expression.NotEqual(value, ConstantNull), (Expression)genericMethod.Invoke(null, new object[] { value, stringValue, this, deep + 1 }));
            }
            else
            {
                throw new NotSupportedException($"Type of {type.FullName} is not supported.");
            }
        }

        public Expression GetNullableExpression(Type type, Expression stringValue, Expression value, ref int deep)
        {
            var hasValueProperty = type.GetProperty("HasValue");
            var valueProperty = type.GetProperty("Value");

            var hasValuePropertyExpression = Expression.Property(value, hasValueProperty);
            var valuePropertyExpressionExpression = Expression.Property(value, valueProperty);

            return Expression.AndAlso(
                hasValuePropertyExpression, // LEFT
                GetExpression(Nullable.GetUnderlyingType(type), stringValue, valuePropertyExpressionExpression, deep)); // RIGHT
        }

        private Expression GetArrayExpression(Type type, Expression filter, Expression array, ref int deep)
        {
            var arrayContentType = type.GetElementType();

            var i = Expression.Parameter(typeof(int), "i");
            var length = Expression.Parameter(typeof(int), "lenght");
            var lengthPropExpression = Expression.Property(array, type.GetProperty("Length"));

            var condition = Expression.LessThan(i, length);
            var value = Expression.ArrayAccess(array, i);

            var loopContent = GetExpression(arrayContentType, filter, value, deep);

            var breakLabel = Expression.Label(typeof(bool));

            var loop = Expression.Block(new[] { i, length },
                Expression.Assign(length, lengthPropExpression),
                Expression.Assign(i, ConstantZero),
                //Expression.Call(Expression.New(typeof(DebugHelper)), ConsoleWriteLine_Int, length),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(i, length),
                        Expression.Block(
                            Expression.IfThen(
                                Expression.IsTrue(loopContent),
                                Expression.Return(breakLabel, ConstantTrue)),
                            Expression.Assign(i, Expression.Add(i, ConstantOne))
                        ),
                        Expression.Block(new[] { i },
                        //Expression.Call(Expression.New(typeof(DebugHelper)), ConsoleWriteLine_Int, ConstantOne),
                        Expression.Return(breakLabel, ConstantFalse)
                        )
                    ),
                breakLabel)
            );

            return Expression.AndAlso(Expression.NotEqual(array, ConstantNull), loop);
        }

        public Expression GetBoolExpression(Expression filter, Expression @bool)
        {
            var callBoolToString = Expression.Call(@bool, BoolToStringMethod);
            return StringContains(callBoolToString, filter);
        }

        public Expression GetByteExpression(Expression filter, Expression @byte)
        {
            var callByteToString = Expression.Call(@byte, ByteToStringMethod);
            return StringContains(callByteToString, filter);
        }

        public Expression GetEnumerableExpression(Expression filter, Expression collection, Type collectionType, ref int deep)
        {
            var genericType = collectionType.GetGenericArguments()[0];
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(genericType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(genericType);

            var value = Expression.Parameter(genericType, "value");

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            var contectOfLoop = GetExpression(genericType, filter, value, deep);

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
            return StringContains(callIntToString, filter);
        }

        public Expression GetStringExpression(Expression filter, Expression str)
        {
            var propIsNotNull = Expression.NotEqual(str, ConstantNull);
            return Expression.AndAlso(propIsNotNull, StringContains(str, filter));
        }

        public Expression GetDateTimeExpression(Expression filter, Expression dateTime)
        {
            var callDateTimeToString = Expression.Call(dateTime, DateTimeToString, ConstantDateTimeToStringFormat);
            return StringContains(callDateTimeToString, filter);
        }
    }
}
