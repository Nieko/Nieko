using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Default Primary Key Filter Expression Provider. Used unless
    /// alternative supplied via a FilterExpressionProvider Attribute
    /// </summary>
    public class DefaultPrimaryKeyExpressionProvider : IPrimaryKeyFilterExpressionProvider
    {
        public static DefaultPrimaryKeyExpressionProvider Instance { get; private set; }

        private DefaultPrimaryKeyExpressionProvider() { }

        static DefaultPrimaryKeyExpressionProvider()
        {
            Instance = new DefaultPrimaryKeyExpressionProvider();
        }

        /// <summary>
        /// Builds a search for instances of <typeparamref name="T"/> where all
        /// properties indicated in <paramref name="key"/> are equal
        /// to their values in <paramref name="key"/>
        /// </summary>
        /// <remarks>
        /// For normal use of the expression it should return zero 
        /// or one instances. Note that <typeparamref name="T"/> is not necessarily
        /// the owning type of <paramref name="key"/>: it may be a type with
        /// identical key fields.
        /// </remarks>
        /// <typeparam name="T">Type that the key belongs to</typeparam>
        /// <param name="key">Unique to key to build search against</param>
        /// <returns>Search expression</returns>
        public Expression<Func<T, bool>> ToFilterExpression<T>(PrimaryKey key)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "o");
            BinaryExpression fieldFilter;
            BinaryExpression fieldsFilter = null;

            foreach (var keyField in key.Keys)
            {
                fieldFilter = Expression.Equal(
                        Expression.Constant(key[keyField], typeof(T)
                            .GetProperty(keyField).PropertyType),
                        Expression.Property(
                            parameter,
                            keyField));
                if (fieldsFilter != null)
                {
                    fieldsFilter = Expression.And(fieldsFilter, fieldFilter);
                }
                else
                {
                    fieldsFilter = fieldFilter;
                }
            }

            return Expression.Lambda<Func<T, bool>>(
                fieldsFilter,
                parameter);
        }
    }
}
