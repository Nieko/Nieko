using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System
{
    public static class TypeExtensions
    {
        private class ByteArrayEqualityComparer : EqualityComparer<byte[]>
        {
            public override bool Equals(byte[] x, byte[] y)
            {
                return x != null && y != null
                            && x.Length == 8 && y.Length == 8
                            && x[0] == y[0]
                            && x[1] == y[1]
                            && x[2] == y[2]
                            && x[3] == y[3]
                            && x[4] == y[4]
                            && x[5] == y[5]
                            && x[6] == y[6]
                            && x[7] == y[7];
            }

            public override int GetHashCode(byte[] obj)
            {
                return obj.GetHashCode();
            }

        }

        private static HashSet<Type> _BoxedTypes = new HashSet<Type>
        {
             typeof(Boolean), 
             typeof(Char), 
             typeof(SByte),
             typeof(Byte),
             typeof(Int16),
             typeof(UInt16), 
             typeof(Int32), 
             typeof(UInt32),
             typeof(Int64), 
             typeof(UInt64),
             typeof(Single),
             typeof(Double),
             typeof(Decimal),
             typeof(DateTime),
             typeof(String)
        };

        private static List<byte[]> frameworkCoreKeyTokens = new List<byte[]>()  
        {
            new byte[] {0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89},
            new byte[] {0x31, 0xbf, 0x38, 0x56, 0xad, 0x36, 0x4e, 0x35},
            new byte[] {0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a}
        };

        /// <summary>
        /// Finds all properties with the Attribute <typeparamref name="T"/> members of type
        /// <paramref name="type"/>
        /// </summary>
        /// <typeparam name="T">Type of attribute to search for</typeparam>
        /// <param name="type">Current instance</param>
        /// <returns>Properties with the attribute</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type)
            where T : Attribute
        {
            HashSet<string> matches = new HashSet<string>();
            HashSet<Type> typesProcessed = new HashSet<Type>();
            IEnumerable<PropertyInfo> properties;

            AddCustomAttributes<T>(type, matches, typesProcessed);

            properties = type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => matches.Contains(p.Name));

            return properties;
        }

        /// <summary>
        /// Attempt to determine if a type is from the core Microsoft
        /// .NET Framework
        /// </summary>
        /// <param name="type">Current instance</param>
        /// <returns>True if Framework type</returns>
        public static bool IsFrameworkType(this Type type)
        {
            if (type == null) { throw new ArgumentNullException("type"); }

            byte[] publicKeyToken = type.Assembly.GetName().GetPublicKeyToken();

            return publicKeyToken != null && publicKeyToken.Length == 8
                && frameworkCoreKeyTokens.Contains(publicKeyToken, new ByteArrayEqualityComparer());
        }

        private static void AddCustomAttributes<T>(Type type, HashSet<string> matches, HashSet<Type> typesProcessed)
            where T : Attribute
        {
            if (typesProcessed.Contains(type))
            {
                return;
            }

            foreach (var match in type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => Attribute.IsDefined(p, typeof(T)) && !matches.Contains(p.Name))
                .Select(p => p.Name))
            {
                matches.Add(match);
            }

            foreach (var parentType in type.BaseType == null ?
                type.GetInterfaces() :
                type.GetInterfaces().Union(new Type[] { type.BaseType }))
            {
                AddCustomAttributes<T>(parentType, matches, typesProcessed);
            }
        }

        /// <summary>
        /// Name of type without namespace stripped of Generic Argument suffix (if any)
        /// </summary>
        /// <param name="Type">Current instance</param>
        /// <returns>Plain text name of Type</returns>
        public static string BasicName(this Type Type)
        {

            string basicName = Type.Name;

            if (basicName.Contains("`"))
            {
                basicName = basicName.Substring(0, basicName.IndexOf('`'));
            }

            return basicName;
        }

        /// <summary>
        /// Returns whether or not a type is 'boxed'
        /// </summary>
        /// <param name="type">Current instance</param>
        /// <returns>True if boxed type</returns>
        public static bool IsBoxedType(this Type type)
        {
            return _BoxedTypes.Contains(type);
        }

        public static Expression<Func<object, object>> ToExpressionFromPath(this Type rootType, string memberPath)
        {
            var rootParameter = Expression.Parameter(typeof(object));

            if(string.IsNullOrEmpty(memberPath))
            {
                return Expression.Lambda<Func<object, object>>(Expression.Convert(rootParameter, typeof(object)), rootParameter);
            }

            var paths = memberPath.Split('.');
            Type currentType = rootType;
            Expression currentExpression = null;

            foreach(var path in paths)
            {
                var member = currentType.GetMember(path).First();
                currentType = (member is PropertyInfo) ?
                    (member as PropertyInfo).PropertyType :
                    (member as FieldInfo).FieldType;

                if(currentExpression == null)
                {
                    currentExpression =
                        Expression.Condition(
                            Expression.Equal(rootParameter, Expression.Constant(null)),
                            Expression.Default(currentType),
                            Expression.PropertyOrField(
                                Expression.Convert(rootParameter, rootType),
                                path));
                }
                else
                {
                    currentExpression = Expression.Condition(
                            Expression.Equal(rootParameter, Expression.Constant(null)),
                            Expression.Constant(currentType),
                            Expression.PropertyOrField(currentExpression, path));
                }
            }

            return Expression.Lambda<Func<object, object>>
                (
                    Expression.Convert(currentExpression, typeof(object)),
                    rootParameter
                );
        }
    
        public static void TouchStaticProperties(this Type type, Type propertyType)
        {
            type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == propertyType)
                .ToList()
                .ForEach(p => p.GetValue(null, null));
        }
    }
}
