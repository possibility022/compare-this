using System;
using System.Linq;
using System.Linq.Expressions;

namespace CompareThis
{
    public class CompareFactory
    {
        public static Expression<Func<T, string, bool>> BuildContainsExpr<T>()
        {
            return BuildContainsExpr<T>(new TypeExpression());
        }

        public static Expression<Func<T, string, bool>> BuildContainsExpr<T>(TypeExpression typeExpression)
        {
            // Declaring parameters
            var parameterSomeClass = Expression.Parameter(typeof(T), "someclass");
            var parameterFilter = Expression.Parameter(typeof(string), "filter");

            // Declaring constant
            var constantNull = Expression.Constant(null);
            var filterIsNotNull = Expression.NotEqual(parameterFilter, constantNull);

            // List of properites
            var prop = typeof(T).GetProperties();

            // Final expressions table.
            var finalPropertyCompare = new Expression[prop.Length];

            for (int i = 0; i < prop.Length; i++)
            {
                var propExpressions = Expression.Property(parameterSomeClass, prop[i]);
                Expression finalExpression = null;

                if (prop[i].PropertyType == typeof(int))
                {
                    // (someclass.IntContains.ToString().Contains(filter))
                    finalExpression = typeExpression.GetExpression(typeof(int), parameterFilter, propExpressions);
                }
                else if (prop[i].PropertyType == typeof(string))
                {
                    // ((someclass.PropA != null) && (someclass.PropA.Contains(filter)))
                    var propIsNotNull = Expression.NotEqual(propExpressions, constantNull);
                    var callContains = Expression.Call(propExpressions, typeExpression.stringContainsMethod, parameterFilter);
                    finalExpression = Expression.AndAlso(propIsNotNull, callContains);
                }
                else if (prop[i].PropertyType == typeof(DateTime?))
                {
                    // ((someclass.DateTimeProp.HasValue)  && (someclass.DateTimeProp.Value.ToString().Contains(filter)))
                    var nullDateTimeProperties = typeof(DateTime?).GetProperties();

                    var hasValueExpression = Expression.Property(
                        propExpressions, nullDateTimeProperties.Where(p => p.Name == "HasValue").First());

                    var valueExpression = Expression.Property(
                        propExpressions, nullDateTimeProperties.Where(p => p.Name == "Value").First());
                    var callDateTimeToString = Expression.Call(valueExpression, typeExpression.dateTimeToString);
                    var contains = Expression.Call(callDateTimeToString, typeExpression.stringContainsMethod, parameterFilter);
                    finalExpression = Expression.AndAlso(hasValueExpression, contains);
                }
                else
                {
                    throw new NotSupportedException($"Type of {prop[i].PropertyType} is not supported.");
                }

                finalPropertyCompare[i] = finalExpression;
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

            final = Expression.AndAlso(filterIsNotNull, final);
            return Expression.Lambda<Func<T, string, bool>>(final, parameterSomeClass, parameterFilter);
        }

        public static Func<T, string, bool> BuildContainsFunc<T>()
        {
            return BuildContainsExpr<T>().Compile();
        }
    }
}
