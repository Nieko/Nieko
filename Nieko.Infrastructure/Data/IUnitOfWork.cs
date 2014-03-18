using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Proxy used by an IDataStoreManager when work against a store is
    /// delegated to it for management. i.e. IDataStoresManager.DoUnitOfWork()
    /// The delegated action uses this proxy to do its CRUD operations.
    /// </summary>
    public interface IUnitOfWork : IDataStore, IDisposable
    {
    }
}
