using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Global object pool for IDataStores and the IModelViewStoresManager
    /// </summary>
    /// <remarks>
    /// The DataStoresManager allows individual actions that make up a Unit
    /// Of Work to occur without knowledge of each other. IDataStore instances
    /// are not released until all their consumers have released.
    /// </remarks>
    public interface IDataStoresManager
    {
        event EventHandler DataStoreClosed;

        IList<Type> DataStoreTypes
        {
            get;
        }

        void CloseAllDataStores();
        void CloseAllDataStores(bool fast);

        IModelViewStoresManager ModelViewStoresManager { get; }

        T GetDataStore<T>(object consumer)
            where T : class, IDataStore;

        void DoUnitOfWork<T>(Action<IDataStore> work)
            where T : class, IDataStore;

        TResult DoUnitOfWork<T, TResult>(Func<IDataStore, TResult> work)
            where T : class, IDataStore;

        void ReleaseDataStore<T>(object consumer)
            where T : class, IDataStore;

        void CloseDataStore<T>(object consumer)
            where T : class, IDataStore;

        /// <summary>
        /// Co-ordinates loading of entities owned by different 
        /// Data Stores, as indicated by a CrossContext attribute
        /// </summary>
        /// <typeparam name="T">Type of entity to cross-load</typeparam>
        /// <param name="entity">Entity with Cross Context reference(s)</param>
        void CrossLoad<T>(T entity);

        /// <summary>
        /// Co-ordinates loading of entities owned by different 
        /// Data Stores, as indicated by a CrossContext attribute
        /// </summary>
        /// <typeparam name="T">Type of entities to cross-load</typeparam>
        /// <param name="entities">Entities with Cross Context reference(s)</param>
        void CrossLoad<T>(IEnumerable<T> entities);
    }
}