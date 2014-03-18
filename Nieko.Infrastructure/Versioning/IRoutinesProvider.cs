using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Builds dictionaries of update routines
    /// </summary>
    public interface IRoutinesProvider
    {
        /// <summary>
        /// For the given component, build a dictionary of all routines for each version,
        /// by version
        /// </summary>
        /// <param name="component">Component to build routines for</param>
        /// <returns>Dictionary of version routines by version</returns>
        IDictionary<Version, IList<IVersionUpdaterRoutine>> CreateComponentRoutines(IVersionedComponent component);
    }
}
