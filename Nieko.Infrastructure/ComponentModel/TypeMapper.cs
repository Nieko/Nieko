using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Static factory class from creating TypeMappers
    /// </summary>
    public abstract class TypeMapper
    {
        /// <summary>
        /// Create Type Mapper by inference from examples. Allows mapping  
        /// anonymous types
        /// </summary>
        /// <typeparam name="TFrom">Type mapping from</typeparam>
        /// <typeparam name="TTo">Type mapping to</typeparam>
        /// <param name="exampleFrom">Example TFrom reference</param>
        /// <param name="exampleTo">Example TTo reference</param>
        /// <returns>TypeMapper for TFrom and TTo</returns>
        public static ITypeMapper<TFrom, TTo> FromExample<TFrom, TTo>(TFrom exampleFrom, TTo exampleTo)
        {
            return new TypeMapper<TFrom, TTo>();
        }

        /// <summary>
        /// Create Type Mapper by inference from examples. Allows mapping  
        /// anonymous types
        /// </summary>
        /// <typeparam name="TFrom">Type mapping from</typeparam>
        /// <typeparam name="TTo">Type mapping to</typeparam>
        /// <param name="examplesFrom">Example TFrom collection</param>
        /// <param name="examplesTo">Example TTo collection</param>
        /// <returns>TypeMapper for TFrom and TTo</returns>
        public static ITypeMapper<TFrom, TTo> FromExamples<TFrom, TTo>(IEnumerable<TFrom> examplesFrom, IEnumerable<TTo> examplesTo)
        {
            return new TypeMapper<TFrom, TTo>();
        }

        /// <summary>
        /// Create two-way Type Mapper
        /// </summary>
        /// <typeparam name="TFrom">Type mapping from</typeparam>
        /// <typeparam name="TTo">Type mapping to</typeparam>
        /// <returns>TypeMapper for TFrom and TTo</returns>
        public static ITypeMapper<TFrom, TTo> New<TFrom, TTo>()
        {
            return new TypeMapper<TFrom, TTo>();
        }

        /// <summary>
        /// Indicates Type Mapper created only copies from. Useful when
        /// TTo has read-only properties that require mirroring from
        /// </summary>
        /// <returns>Fluent from-only Type Mapper factory</returns>
        public static ITypeMapperFromOnlyFactory FromOnly()
        {
            return new TypeMapperFromOnlyFactory();
        }

        /// <summary>
        /// Indicates Type Mapper created only copies to. Useful when
        /// TFrom has read-only properties that require mirroring from
        /// </summary>
        /// <returns>Fluent to-only Type Mapper factory</returns>
        public static ITypeMapperToOnlyFactory ToOnly()
        {
            return new TypeMapperToOnlyFactory();
        }
    }
}
