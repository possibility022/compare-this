using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CompareThis
{
    public class CompareFactory
    {
        public static Expression BuildContainsExpr<T>(
            Expression parameterSomeClass,
            Expression parameterFilter,
            Settings settings = null,
            bool checkFilter = false)
        {
            // Declaring constant
            var constantNull = Expression.Constant(null);
            var filterIsNotNull = Expression.NotEqual(parameterFilter, constantNull);

            var typeExpression = new TypeExpression(settings);

            var contains = BuildContainsExpr<T>(parameterSomeClass, parameterFilter, typeExpression, 1);

            if (checkFilter)
                return Expression.AndAlso(filterIsNotNull, contains);
            else
                return contains;
        }

        public static Expression BuildContainsExpr<T>(
            Expression parameterSomeClass,
            Expression parameterFilter,
            TypeExpression typeExpression,
            int deep)
        {
            // List of properites
            var prop = typeof(T).GetProperties();

            // Final expressions table.
            var finalPropertyCompare = new List<Expression>();

            for (int i = 0; i < prop.Length; i++)
            {
                var propExpressions = Expression.Property(parameterSomeClass, prop[i]);

                var expression = typeExpression.GetExpression(
                        prop[i].PropertyType, parameterFilter, propExpressions, deep);
                if (expression != null)
                {
                    finalPropertyCompare.Add(
                        typeExpression.WriteLineWrapper_Debugger(expression
                            , prop[i].PropertyType, prop[i].Name));
                }
            }

            Expression final;

            if (finalPropertyCompare.Count == 1)
            {
                final = finalPropertyCompare[0];
            }
            else
            {
                final = Expression.OrElse(finalPropertyCompare[1], finalPropertyCompare[0]);

                if (finalPropertyCompare.Count > 2)
                {
                    for (int i = 2; i < finalPropertyCompare.Count; i++)
                    {
                        final = Expression.OrElse(final, finalPropertyCompare[i]);
                    }
                }
            }

            return final;
        }

        public static Func<T, string, bool> BuildContainsFunc<T>(Settings settings = null)
        {
            // Declaring parameters
            var parameterSomeClass = Expression.Parameter(typeof(T), "someclass");
            var parameterFilter = Expression.Parameter(typeof(string), "filter");
            var final = BuildContainsExpr<T>(parameterSomeClass, parameterFilter, settings, true);
            return Expression.Lambda<Func<T, string, bool>>(final, parameterSomeClass, parameterFilter).Compile();
        }
    }
}
