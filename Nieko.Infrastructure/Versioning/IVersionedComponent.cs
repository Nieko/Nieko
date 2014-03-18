using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Versioned Component
    /// </summary>
    [PluginDependancies(typeof(IDataStoreRegistration))]
    public interface IVersionedComponent
    {
        string Name { get; }
    }
}