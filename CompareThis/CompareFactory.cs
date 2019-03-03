using System;
using System.Linq.Expressions;

namespace CompareThis
{
    public class CompareFactory
    {
        public static Expression BuildContainsExpr<T>(
            Expression parameterSomeClass,
            Expression parameterFilter,
            bool checkFilter)
        {
            // Declaring constant
            var constantNull = Expression.Constant(null);
            var filterIsNotNull = Expression.NotEqual(parameterFilter, constantNull);

            var contains = BuildContainsExpr<T>(parameterSomeClass, parameterFilter);

            if (checkFilter)
                return Expression.AndAlso(filterIsNotNull, contains);
            else
                return contains;
        }

        public static Expression BuildContainsExpr<T>(
            Expression parameterSomeClass,
            Expression parameterFilter)
        {
            // List of properites
            var prop = typeof(T).GetProperties();

            // Final expressions table.
            var finalPropertyCompare = new Expression[prop.Length];

            for (int i = 0; i < prop.Length; i++)
            {
                var propExpressions = Expression.Property(parameterSomeClass, prop[i]);
                finalPropertyCompare[i] = TypeExpression
                    .Instance.WriteLineWrapper_Debugger(
                    TypeExpression.Instance.GetExpression(
                        prop[i].PropertyType, parameterFilter, propExpressions)
                        , prop[i].PropertyType, prop[i].Name);
            }

            Expression final;

            if (finalPropertyCompare.Length == 1)
            {
                final = finalPropertyCompare[0];
            }
            else
            {
                final = Expression.OrElse(finalPropertyCompare[1], finalPropertyCompare[0]);

                if (finalPropertyCompare.Length > 2)
                {
                    for (int i = 2; i < finalPropertyCompare.Length; i++)
                    {
                        final = Expression.OrElse(final, finalPropertyCompare[i]);
                    }
                }
            }

            return final;
        }

        public static Func<T, string, bool> BuildContainsFunc<T>()
        {
            // Declaring parameters
            var parameterSomeClass = Expression.Parameter(typeof(T), "someclass");
            var parameterFilter = Expression.Parameter(typeof(string), "filter");
            var final = BuildContainsExpr<T>(parameterSomeClass, parameterFilter);
            return Expression.Lambda<Func<T, string, bool>>(final, parameterSomeClass, parameterFilter).Compile();
        }
    }
}
