using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Declares that <see cref="Type"/> ModelView Stores should be
    /// made available and that they are implementations of <see cref="BaseStoreType"/>
    /// to be used from <see cref="UsedFrom"/>
    /// </summary>
    [PluginDependancies(typeof(IDataStoreRegistration))] 
    public interface IModelViewStoreRegistration
    {
        Type Type { get; }
        Type BaseStoreType { get; }
        DateTime UsedFrom { get; }
        Action<IModelViewStore, DateTime> SetDateAsAt { get; }
    }
}
