using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Infrastructure.Linq.Expressions
{
    /// <summary>
    /// Decomposes an Expression Tree into details useful for
    /// transversal and comparison
    /// </summary>
    /// <typeparam name="T">Expression Type Parameter</typeparam>
    public class ExpressionInfo<T>
    {
        private Expression<T> _Expression;

        public ExpressionInfo(Expression<T> expression)
        {
            Expression = expression;
        }

        public bool ContainsMethodCall
        {
            get
            {
                return TreeContainsMethodCall(Expression.Body);
            }
        }

        public Expression<T> Expression
        {
            get
            {
                return _Expression;
            }
            private set
            {
                _Expression = value;
            }
        }

        public IList<object> GetParameters()
        {
            List<object> parameters = new List<object>();

            GetTreeParameters(Expression.Body, parameters);

            return parameters;
        }

        private void GetTreeParameters(Expression exp, IList<object> parameters)
        {
            if (exp == null)
            {
                return;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    GetTreeParameters(((UnaryExpression)exp).Operand, parameters);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    GetTreeParameters(((BinaryExpression)exp).Left, parameters);
                    GetTreeParameters(((BinaryExpression)exp).Conversion, parameters);
                    GetTreeParameters(((BinaryExpression)exp).Right, parameters);
                    break;
                case ExpressionType.TypeIs:
                    GetTreeParameters(((TypeBinaryExpression)exp).Expression, parameters);
                    break;
                case ExpressionType.Conditional:
                    GetTreeParameters(((ConditionalExpression)exp).IfFalse, parameters);
                    GetTreeParameters(((ConditionalExpression)exp).IfTrue, parameters);
                    GetTreeParameters(((ConditionalExpression)exp).Test, parameters);
                    break;
                case ExpressionType.Constant:
                    parameters.Add(((ConstantExpression)exp).Value);
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.MemberAccess:
                    GetTreeParameters(((MemberExpression)exp).Expression, parameters);
                    break;
                case ExpressionType.Call:
                    break;
                case ExpressionType.Lambda:
                    GetTreeParameters(((LambdaExpression)exp).Body, parameters);
                    foreach (ParameterExpression item in ((LambdaExpression)exp).Parameters)
                    {
                        GetTreeParameters(item, parameters);
                    }
                    break;
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Invoke:
                case ExpressionType.MemberInit:
                case ExpressionType.ListInit:
                    break;
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        private bool TreeContainsMethodCall(Expression exp)
        {
            if (exp == null)
            {
                return false;
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return TreeContainsMethodCall(((UnaryExpression)exp).Operand);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return TreeContainsMethodCall(((BinaryExpression)exp).Left) ||
                        TreeContainsMethodCall(((BinaryExpression)exp).Conversion) ||
                        TreeContainsMethodCall(((BinaryExpression)exp).Right);
                case ExpressionType.TypeIs:
                    return TreeContainsMethodCall(((TypeBinaryExpression)exp).Expression);
                case ExpressionType.Conditional:
                    return TreeContainsMethodCall(((ConditionalExpression)exp).IfFalse) ||
                        TreeContainsMethodCall(((ConditionalExpression)exp).IfTrue) ||
                        TreeContainsMethodCall(((ConditionalExpression)exp).Test);
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                    return false;
                case ExpressionType.MemberAccess:
                    return TreeContainsMethodCall(((MemberExpression)exp).Expression);
                case ExpressionType.Call:
                    return true;
                case ExpressionType.Lambda:
                    return TreeContainsMethodCall(((LambdaExpression)exp).Body)
                        || ((LambdaExpression)exp).Parameters.Any(p=> TreeContainsMethodCall(p));
                case ExpressionType.New:
                    return true;
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return true;
                case ExpressionType.Invoke:
                    return true;
                case ExpressionType.MemberInit:
                    return true;
                case ExpressionType.ListInit:
                    return true;
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

    }
}