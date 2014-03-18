using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Provides all versioned components in the application by name
    /// </summary>
    public interface IComponentsProvider
    {
        IDictionary<string, IVersionedComponent> Components { get; }
    }
}