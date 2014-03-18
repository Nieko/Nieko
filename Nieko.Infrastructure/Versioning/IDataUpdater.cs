using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Update handler for an individual versioned component.
    /// Saves and loads version information from data store.
    /// <remarks>
    /// Uses System.Version to represent different versions
    /// </remarks>
    /// </summary>
    public interface IDataUpdater
    {
        /// <summary>
        /// Component this updater runs for
        /// </summary>
        IVersionedComponent Component { get; }
        /// <summary>
        /// All versions pertaining to this component, sorted by version number
        /// </summary>
        SortedList<Version, Version> Versions { get; }
        /// <summary>
        /// Returns the current version of the component
        /// </summary>
        /// <returns>Component version</returns>
        Version GetDataVersion();
        /// <summary>
        /// Changes the component version
        /// </summary>
        /// <param name="version">New Version</param>
        void SaveVersion(Version version);
        /// <summary>
        /// Performs all updates required to bring the components
        /// current version to the specified version
        /// </summary>
        /// <param name="toVersion">Version to update to</param>
        void UpdateData(Version toVersion);
    }
}