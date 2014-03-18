using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Linq.Expressions;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure.Logging;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// base class implementing IDataStoresManager. Intended as starting point for 
    /// implementation of IDataStoresManager.
    /// </summary>
    public abstract class DataStoresManager : IDataStoresManager
    {
        private IList<Type> _DataStoresTypes;
        private Dictionary<IDataStore, HashSet<object>> _DataStoreConsumers = new Dictionary<IDataStore, HashSet<object>>();
        private CrossContextLoader _CrossContextLoader;

        public event EventHandler DataStoreClosed;
        private Func<Type, IDataStore> _StoreSupplier;

        public IList<Type> DataStoreTypes
        {
            get
            {
                return _DataStoresTypes;
            }
            protected set
            {
                _DataStoresTypes = value;
                
                DataStoresByType.Clear();

                foreach (var type in _DataStoresTypes)
                {
                    DataStoresByType.Add(type, null);
                }
            }
        }

        public IModelViewStoresManager ModelViewStoresManager { get; protected set; }

        protected Dictionary<Type, IDataStore> DataStoresByType { get; private set; }

        public DataStoresManager(Func<Type, IDataStore> storeSupplier)
        {
            DataStoresByType = new Dictionary<Type, IDataStore>();
            _CrossContextLoader = new CrossContextLoader(this);
            _StoreSupplier = storeSupplier;
        }

        public void CloseAllDataStores()
        {
            CloseAllDataStores(false);
        }

        public void CloseAllDataStores(bool fast)
        {
            if (!fast)
            {
                foreach (var repository in DataStoresByType.Values)
                {
                    OnDataStoreClosed(repository);
                }
            }

            _DataStoreConsumers.Clear();
            DataStoresByType.Clear();
        }

        public T GetDataStore<T>(object consumer)
            where T : class, IDataStore
        {
            IDataStore dataStore;

            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }
            
            if (!DataStoresByType.TryGetValue(typeof(T), out dataStore))
            {
                throw new KeyNotFoundException("DataStoresManager does not manage DataStores of type " + typeof(T));
            }

            lock (_DataStoreConsumers)
            {
                if (dataStore == null)
                {
                    Logger.Instance.Log("DataStore " + typeof(T).FullName + " requested : new instance opened");
                    dataStore = _StoreSupplier(typeof(T));
                    DataStoresByType[typeof(T)] = dataStore;
                    _DataStoreConsumers.Add(dataStore, new HashSet<object>());
                }
                else
                {
                    Logger.Instance.Log("DataStore " + typeof(T).FullName + " requested : re-using existing instance");
                }
                _DataStoreConsumers[dataStore].Add(consumer);
            }

            return dataStore as T;
        }

        public void DoUnitOfWork<T>(Action<IDataStore> work)
            where T : class, IDataStore
        {
            using (var unitOfWork = new UnitOfWork(this))
            {
                unitOfWork.Begin<T>();

                work(unitOfWork);
            }
        }

        public TResult DoUnitOfWork<T, TResult>(Func<IDataStore, TResult> work)
            where T : class, IDataStore
        {
            TResult result;

            using (var unitOfWork = new UnitOfWork(this))
            {
                unitOfWork.Begin<T>();

                result = work(unitOfWork);
            }

            return result;
        }

        public void ReleaseDataStore<T>(object consumer)
            where T : class, IDataStore
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            if (!DataStoresByType.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("DataStoresManager does not manage DataStores of type " + typeof(T));
            }

            IDataStore dataStore = DataStoresByType[typeof(T)];

            Logger.Instance.Log("DataStore " + typeof(T).FullName + " released by " + consumer.GetType().FullName);
            _DataStoreConsumers[dataStore].Remove(consumer);

            if (_DataStoreConsumers[dataStore].Count == 0)
            {
                Logger.Instance.Log("DataStore " + typeof(T).FullName + " has no consumers : closing");
                OnDataStoreClosed(dataStore);
                if (dataStore is IDisposable)
                {
                    (dataStore as IDisposable).Dispose();
                }
                _DataStoreConsumers.Remove(dataStore);
                DataStoresByType[typeof(T)] = null;
                OnDataStoreClosed(dataStore);
            }
        }

        public void CloseDataStore<T>(object consumer) where T : class, IDataStore
        {
            ReleaseDataStore<T>(consumer);

            var dataStore = DataStoresByType[typeof(T)];

            if (dataStore != null)
            {
                throw new InvalidOperationException("Cannot close Data Store " + typeof(T).BasicName() + ": Data Store is still in use.");  
            }
        }

        private void OnDataStoreClosed(IDataStore dataStore)
        {
            EventHandler handler = DataStoreClosed;
            if (handler != null)
            {
                handler(dataStore, new EventArgs());
            }
        }

        public void CrossLoad<T>(T entity)
        {
            _CrossContextLoader.CrossLoad(entity);
        }

        public void CrossLoad<T>(IEnumerable<T> entities)
        {
            _CrossContextLoader.CrossLoad(entities);
        }
    }
}