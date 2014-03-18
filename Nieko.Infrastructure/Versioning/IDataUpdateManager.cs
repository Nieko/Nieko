using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Determines if any versioned component in the application
    /// requires an update to data and performs a managed update
    /// to all components throughout the application to their current
    /// version
    /// </summary>
    public interface IDataUpdateManager
    {
        IComponentsProvider ComponentsProvider { get; }
        bool UpdateRequired { get; }
        void UpdateToCurrentVersion();
    }
}