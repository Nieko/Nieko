using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Declares to DataStoresManager that <see cref="Type"/>
    /// is an IDataStore and should be made available
    /// </summary>
    public interface IDataStoreRegistration
    {
        /// <summary>
        /// IDataStore type. Must implement IDataStore
        /// </summary>
        Type Type { get; }
        string ConnectionStringName { get; }
    }
}
