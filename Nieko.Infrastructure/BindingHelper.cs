using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure
{
    /// <summary>
    /// Static helper to provide compile-time checked property name strings via
    /// Expressions
    /// </summary>
    public static class BindingHelper
    {
        public static string Name<T, T2>(Expression<Func<T, T2>> expression)
        {
            return GetMemberName(expression.Body);
        }

        public static string Name<T>(Expression<Func<T>> expression)
        {
            return GetMemberName(expression.Body);
        }

        public static string Name<T>(Expression<Action<T>> expression)
        {
            return GetMemberName(expression);
        }

        private static string GetMemberName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var supername = memberExpression.Expression == null ? string.Empty : GetMemberName(memberExpression.Expression);
                    if (String.IsNullOrEmpty(supername)) return memberExpression.Member.Name;
                    return String.Concat(supername, '.', memberExpression.Member.Name);
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;
                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetMemberName(unaryExpression.Operand);
                case ExpressionType.Parameter:
                case ExpressionType.Constant:
                    return String.Empty;
                case ExpressionType.ArrayIndex:
                    var arrayIndexExpression = (BinaryExpression)expression;
                    return GetMemberName(arrayIndexExpression.Left) + "[" + GetMemberName(arrayIndexExpression.Right) + "]";
                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }

        /// <summary>
        /// Converts property accessor to Getter
        /// </summary>
        /// <typeparam name="T">Class with property</typeparam>
        /// <typeparam name="T2">Return type of property</typeparam>
        /// <param name="expression">Property accessor</param>
        /// <returns>Getter as Function delegate</returns>
        public static Func<T, T2> GetGetter<T, T2>(Expression<Func<T, T2>> expression)
        {
            return expression.Compile();
        }
        
        /// <summary>
        /// Converts property accessor to Setter
        /// </summary>
        /// <typeparam name="T">Class with property</typeparam>
        /// <typeparam name="T2">Return type of property</typeparam>
        /// <param name="expression">Property accessor</param>
        /// <returns>Setter as Action delegate</returns>
        public static Action<T, T2> GetSetter<T, T2>(Expression<Func<T, T2>> expression)
        {
            var instanceParameter = Expression.Parameter(typeof(T), "o");
            var valueParameter = Expression.Parameter(typeof(T2), "v");

            return Expression.Lambda<Action<T, T2>>(
                Expression.Call(
                    instanceParameter,
                    typeof(T).GetProperty(BindingHelper.Name(expression)).GetSetMethod(),
                    valueParameter),
                instanceParameter,
                valueParameter).Compile();
        }
    
    }
}