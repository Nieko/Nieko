using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq.Expressions
{
    public static class Extensions
    {
        /// <summary>
        /// Builds a predicate demanding that <typeparamref name="TElement"/> items have <typeparamref name="TValue"/> values
        /// within the enumeration <paramref name="values"/>
        /// </summary>
        /// <typeparam name="TElement">Item type filtering</typeparam>
        /// <typeparam name="TValue">Type of value filtering by</typeparam>
        /// <typeparam name="TQueryValue">Result of query build thus far</typeparam>
        /// <param name="query">Current instance</param>
        /// <param name="valueSelector">Filtered value accessor</param>
        /// <param name="values">Values to filter for</param>
        /// <returns>Filter expression</returns>
        public static Expression<Func<TElement, bool>> BuildContains<TElement, TValue, TQueryValue>(
            this Expression<Func<TElement, TQueryValue>> query,
            Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }
            ParameterExpression p = valueSelector.Parameters.Single();
            // p => valueSelector(p) == values[0] || valueSelector(p) == ...
            if (!values.Any())
            {
                return e => false;
            }
            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            var result = Expression.Lambda<Func<TElement, bool>>(body, p);

            var invocation = Expression.Invoke(result, query.Parameters.Cast<Expression>());
            result = Expression.Lambda<Func<TElement, bool>>(Expression.OrElse(query.Body, invocation), query.Parameters);

            return result;
        }
    }
}
