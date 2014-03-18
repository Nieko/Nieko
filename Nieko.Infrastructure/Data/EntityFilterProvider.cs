using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Objects.DataClasses;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Primary Key Filter Expression Provider with special consideration for 
    /// Entities
    /// </summary>
    public class EntityFilterProvider : IPrimaryKeyFilterExpressionProvider
    {
        private static Dictionary<Type, Dictionary<string, object>> _PropertyExpressionsByType = new Dictionary<Type, Dictionary<string, object>>();

        public Expression<Func<T, bool>> ToFilterExpression<T>(PrimaryKey key)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "o");
            Expression fieldFilter;
            Expression fieldsFilter = null;

            foreach (var keyField in key.Keys)
            {
                fieldFilter = GetPropertyExpressionBuilder<T>(keyField)(key, parameter);

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

        private Func<PrimaryKey, ParameterExpression, Expression> GetPropertyExpressionBuilder<T>(string property)
        {
            Dictionary<string, object> propertyExpressions = null;
            object expression;

            if (!_PropertyExpressionsByType.TryGetValue(typeof(T), out propertyExpressions))
            {
                propertyExpressions = new Dictionary<string, object>();
                _PropertyExpressionsByType.Add(typeof(T), propertyExpressions);
            }

            if (propertyExpressions.TryGetValue(property, out expression))
            {
                return expression as Func<PrimaryKey, ParameterExpression, Expression>;
            }

            var propertyInfo = typeof(T).GetProperty(property);
            var propertyType = propertyInfo.PropertyType;
            Func<PrimaryKey, ParameterExpression, Expression> newExpression = null;

            if (typeof(EntityObject).IsAssignableFrom(propertyType))
            {
                ParameterExpression keyPropertyExpression = Expression.Parameter(typeof(object), "ref");
                var IdAccessor = Expression.Lambda<Func<object, int>>(
                        Expression.Property(
                            Expression.Convert(
                                keyPropertyExpression,
                                propertyType),
                            "Id"),
                        keyPropertyExpression)
                    .Compile();

                newExpression = (PrimaryKey key, ParameterExpression parameter) =>
                {
                    if (key[property] == null)
                    {
                        return Expression.Equal(
                                Expression.Constant(null, propertyType),
                                Expression.Property(
                                        parameter,
                                        property));
                    }
                    else
                    {
                        return Expression.Equal(
                                Expression.Constant(IdAccessor(key[property]), typeof(int)),
                                Expression.Property(
                                    Expression.Property(
                                        parameter,
                                        property),
                                    "Id"));
                    }
                };
            }
            else
            {
                newExpression = (PrimaryKey key, ParameterExpression parameter) =>
                    {
                        object value = key[property];

                        if (propertyType == typeof(DateTime))
                        {
                            if (((DateTime)value) < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                            {
                                value = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                            }
                            else if (((DateTime)value) > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            {
                                value = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;
                            }
                        }

                        return Expression.Equal(
                            Expression.Constant(value, propertyType),
                            Expression.Property(
                                parameter,
                                property));
                    };
            }

            propertyExpressions.Add(property, newExpression);

            return newExpression;
        }
    }
}
