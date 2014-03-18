using System;
using System.Collections.Generic;
namespace Nieko.Infrastructure.ComponentModel
{
    public interface ITypeMapperToOnlyFactory
    {
        ITypeMapper<TFrom, TTo> FromExample<TFrom, TTo>(TFrom exampleFrom, TTo exampleTo);
        ITypeMapper<TFrom, TTo> FromExamples<TFrom, TTo>(IEnumerable<TFrom> examplesFrom, IEnumerable<TTo> examplesTo);
        ITypeMapper<TFrom, TTo> New<TFrom, TTo>();
    }
}
