using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Provides a delayed resolution of a type.
    /// /// </summary>
    /// <remarks>
    /// Particularly useful in breaking circular
    /// dependencies between classes or in ensuring
    /// that resources have been initialized before creating
    /// an object.
    /// </remarks>
    public interface IGenericSupplierBuilder
    {
        Func<T> BuildSupplier<T>();
        Func<object> BuildSupplier(Type type);
    }
}
