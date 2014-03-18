using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Primary Key Filter Expression Provider
    /// </summary>
    public interface IPrimaryKeyFilterExpressionProvider
    {
        // <summary>
        /// Builds a search for instances of <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// Not used directly but called from PrimaryKey.ToFilterExpression.
        /// 
        /// For normal use of the expression it should return zero 
        /// or one instances. Note that <typeparamref name="T"/> is not necessarily
        /// the owning type of <paramref name="key"/>: it may be a type with
        /// identical key fields.
        /// </remarks>
        /// <typeparam name="T">Type that the key belongs to</typeparam>
        /// <param name="key">Unique to key to build search against</param>
        /// <returns>Search expression</returns>
        Expression<Func<T, bool>> ToFilterExpression<T>(PrimaryKey key);
    }
}
