using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    public enum ConnectionDetailsFormat
    {
        None,
        ConfigName,
        ConnectionString
    }

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
        /// <summary>
        /// Indicates what information is provided by <see cref="ConnectionDetails"/>.
        /// Should return <see cref="ConnectionDetailsFormat.None"/> if no 
        /// details are provided
        /// </summary>
        ConnectionDetailsFormat ConnectionType { get; }
        /// <summary>
        /// Details required to connect to IDataStore. Nature of contents
        /// should be indicated by <see cref="ConnectionType"/>
        /// </summary>
        string ConnectionDetails { get; }
    }
}
