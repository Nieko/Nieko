using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class TypeMapperFromOnlyFactory : ITypeMapperFromOnlyFactory
    {
        public ITypeMapper<TFrom, TTo> FromExample<TFrom, TTo>(TFrom exampleFrom, TTo exampleTo)
        {
            return new TypeMapper<TFrom, TTo>()
            {
                ToReadOnly = true
            };
        }

        public ITypeMapper<TFrom, TTo> FromExamples<TFrom, TTo>(IEnumerable<TFrom> examplesFrom, IEnumerable<TTo> examplesTo)
        {
            return new TypeMapper<TFrom, TTo>()
            {
                ToReadOnly = true
            };
        }

        public ITypeMapper<TFrom, TTo> New<TFrom, TTo>()
        {
            return new TypeMapper<TFrom, TTo>()
            {
                ToReadOnly = true
            };
        }
    }
}
