using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure;

namespace System.Linq
{
    public static class Extensions
    {
        /// <summary>
        /// Builds a predicate demanding that <typeparamref name="TElement"/> items have <typeparamref name="TValue"/> values
        /// within the enumeration <paramref name="values"/>
        /// </summary>
        /// <typeparam name="TElement">Item type filtering</typeparam>
        /// <typeparam name="TValue">Type of value filtering by</typeparam>
        /// <param name="query">Current instance</param>
        /// <param name="valueSelector">Filtered value accessor</param>
        /// <param name="values">Values to filter for</param>
        /// <returns>Filter expression</returns>
        public static Expression<Func<TElement, bool>> BuildContains<TElement, TValue>(
            this IEnumerable<TElement> query,
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
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        /// <summary>
        /// Builds a predicate demanding that <typeparamref name="TElement"/> items have <typeparamref name="TValue"/> values
        /// not within the enumeration <paramref name="values"/>
        /// </summary>
        /// <typeparam name="TElement">Item type filtering</typeparam>
        /// <typeparam name="TValue">Type of value filtering by</typeparam>
        /// <param name="query">Current instance</param>
        /// <param name="valueSelector">Filtered value accessor</param>
        /// <param name="values">Values to filter out</param>
        /// <returns>Filter expression</returns>
        public static Expression<Func<TElement, bool>> BuildExclude<TElement, TValue>(
            this IEnumerable<TElement> query,
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
            var notEquals = values.Select(value => (Expression)Expression.NotEqual(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = notEquals.Aggregate<Expression>((accumulate, notEqual) => Expression.AndAlso(accumulate, notEqual));
            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }

        /// <summary>
        /// Converts Expression representing property accessor to property setter
        /// </summary>
        /// <typeparam name="TOwner">Class with property as member</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="getter">Property accessor</param>
        /// <returns>Property setter</returns>
        public static Expression<Action<TOwner, T>> ToSetter<TOwner, T>(this Expression<Func<TOwner, T>> getter)
        {
            var ownerParameter = Expression.Parameter(typeof(TOwner), "o");
            var valueParameter = Expression.Parameter(typeof(T), "v");

            return Expression.Lambda<Action<TOwner, T>>(
                Expression.Call(
                    ownerParameter,
                    typeof(TOwner).GetProperty(BindingHelper.Name(getter)).GetSetMethod(),
                    valueParameter),
                ownerParameter,
                valueParameter);
        }
    }
}