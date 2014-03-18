using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.Linq
{
    public static class TypeExtensions
    {
        private static Dictionary<Type, object> _BoxedTypeDefaults = new Dictionary<Type, object>()
        {
            { typeof(Boolean), default(Boolean) },
            { typeof(Char), default(Char) },
            { typeof(SByte), default(SByte)},
            { typeof(Byte), default(Byte) },
            { typeof(Int16), default(Int16) },
            { typeof(UInt16), default(UInt16) },
            { typeof(Int32), default(Int32) },
            { typeof(UInt32), default(UInt32) },
            { typeof(Int64), default(Int64) },
            { typeof(UInt64), default(UInt64) },
            { typeof(Single), default(Single) },
            { typeof(Double), default(Double) },
            { typeof(Decimal), default(Decimal) },
            { typeof(DateTime), default(DateTime) },
            { typeof(String), string.Empty }                // string requires special consideration as default(string) is null
        };

        /// <summary>
        /// Creates ConstantExpression having the value as the default for the type
        /// </summary>
        /// <remarks>
        /// - Value types return constructed value
        /// - Boxed Types return their default()
        /// - All else returns null
        /// - string / String is handled explicitly to return string.Empty
        /// </remarks>
        /// <param name="type">Current instance</param>
        /// <returns>ConstantExpression with default value</returns>
        public static ConstantExpression AsDefaultExpression(this Type type)
        {
            if (type == typeof(string))
            {
                return Expression.Constant(string.Empty);
            }

            if (type.IsValueType)
            {
                return Expression.Constant(Activator.CreateInstance(type));
            }
            else if (_BoxedTypeDefaults.ContainsKey(type))
            {
                return Expression.Constant(_BoxedTypeDefaults[type]);
            }
            else
            {
                return Expression.Constant(null, type);
            }
        }
    }
}
