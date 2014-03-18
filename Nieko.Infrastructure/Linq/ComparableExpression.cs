using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Linq.Expressions;

namespace Nieko.Infrastructure.Linq
{
    /// <summary>
    /// Wraps an expression so that it may be compared with another expressions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComparableExpression<T> : IComparable<ComparableExpression<T>>, IComparable
    {
        private static long _LastOrdinal = long.MinValue;

        private string _Body;
        private long _Ordinal;
        private IList<object> _Parameters;

        public ComparableExpression(Expression<T> method)
        {
            if (_LastOrdinal == long.MaxValue)
            {
                _LastOrdinal = long.MinValue;
            }
            _LastOrdinal++;
            _Ordinal = _LastOrdinal;
            Body = method.Body.ToString();
            Parameters = new ExpressionInfo<T>(method).GetParameters();
        }

        public string Body
        {
            get
            {
                return _Body;
            }
            private set
            {
                _Body = value;
            }
        }

        public IList<object> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                _Parameters = value;
            }
        }

        /// <summary>
        /// Compares two expressions by comparing their decompiled expression body.
        /// Parameter count and types are checked first and if different an internal comparison
        /// based on which ComparibleExpression[T] was created first is used
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ComparableExpression<T> other)
        {
            IEnumerator<object> selfEnumerator;
            IEnumerator<object> otherEnumerator;

            if (other.Parameters.Count != Parameters.Count)
            {
                return _Ordinal.CompareTo(other._Ordinal);
            }
            selfEnumerator = Parameters.GetEnumerator();
            otherEnumerator = other.Parameters.GetEnumerator();

            while (selfEnumerator.MoveNext())
            {
                otherEnumerator.MoveNext();
                if (selfEnumerator.Current != otherEnumerator.Current)
                {
                    return _Ordinal.CompareTo(other._Ordinal);
                }
            }

            return Body.CompareTo(other.Body);
        }

        public int CompareTo(object obj)
        {
            return CompareTo((ComparableExpression<T>)obj);
        }

    }
}