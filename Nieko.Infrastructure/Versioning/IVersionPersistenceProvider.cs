using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Persistence implementation for a
    /// versioned component
    /// </summary>
    public interface IVersionPersistenceProvider
    {
        /// <summary>
        /// Finds what the current version is from the
        /// database
        /// </summary>
        /// <returns></returns>
        Version GetDataVersion();
        /// <summary>
        /// Saves versioning information to database
        /// </summary>
        /// <param name="version"></param>
        void SaveVersion(Version version);
    }
}
