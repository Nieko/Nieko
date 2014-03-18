using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Reflection;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Routine to update a versioned component to a particular
    /// version.
    /// </summary>
    /// <remarks>
    /// The update for each version of each component requires a 
    /// separate class implementing this interface
    /// </remarks>
    [PluginDependancies(typeof(IDataStoreRegistration))]
    public interface IVersionUpdaterRoutine
    {
        /// <summary>
        /// Component to update
        /// </summary>
        IVersionedComponent Component { get; }
        /// <summary>
        /// Specific updates that must be applied first before this
        /// update
        /// </summary>
        /// <remarks>
        /// The list contains the types implementing IVersionUpdaterRoutine that must be 
        /// applied before this update. Depended Updates may be for other components but must
        /// be for the same version as this and for the same Data Store
        /// </remarks>
        IList<Type> DependsOn { get; }
        /// <summary>
        /// Version to which this update will bring the component up to
        /// </summary>
        Version Version { get; }
        /// <summary>
        /// Runs the update
        /// </summary>
        void ApplyDataUpdate();
    }
}