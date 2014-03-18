using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Facade for global access to Settings instances
    /// </summary>
    public interface ISettingsProvider
    {
        T GetSettings<T>() where T : class, ISettings;
    }
}
