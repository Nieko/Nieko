using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Composition;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// IDataStoresManager implementation that synchronizes the creation and release
    /// of IDataStores
    /// </summary>
    public abstract class SynchronizedDataStoresManager : IDataStoresManager
    {
        private class DataStoresManagerInternal : DataStoresManager 
        {
            public DataStoresManagerInternal(Func<Type, IDataStore> storeSupplier)
                : base(storeSupplier)
            {
            }

            internal void SetDataStoreTypes(IList<Type> dataStoreTypes)
            {
                DataStoreTypes = dataStoreTypes;
            }
        }

        private object _LockObject = new object();
        private DataStoresManagerInternal _Instance;
        private IModelViewStoresManager _ModelViewStoresManager;
        private Func<IModelViewStoresManager> _ModelViewStoresManagerFactory;

        public event EventHandler DataStoreClosed
        {
            add
            {
                DoLocked(()=>
                {
                    _Instance.DataStoreClosed += value;
                });
            }
            remove
            {
                DoLocked(() =>
                {
                    _Instance.DataStoreClosed -= value;
                });
            }
        }

        public IList<Type> DataStoreTypes
        {
            get 
            {
                return GetLocked(() => { return _Instance.DataStoreTypes; });
            }
            protected set
            {
                DoLocked(() => _Instance.SetDataStoreTypes(value));
            }
        }

        public IModelViewStoresManager ModelViewStoresManager 
        {
            get
            {
                if (_ModelViewStoresManager == null)
                {
                    _ModelViewStoresManager = _ModelViewStoresManagerFactory();
                }
                return _ModelViewStoresManager;
            }
        }

        public SynchronizedDataStoresManager(Func<Type, IDataStore> storeSupplier, Func<IModelViewStoresManager> modelViewStoresManagerFactory)
        {
            _ModelViewStoresManagerFactory = modelViewStoresManagerFactory;
            _Instance = new DataStoresManagerInternal(storeSupplier);
        }

        public void CloseAllDataStores()
        {
            CloseAllDataStores(false);
        }

        public void CloseAllDataStores(bool fast)
        {
            DoLocked(() => _Instance.CloseAllDataStores(fast));
        }

        public T GetDataStore<T>(object consumer) where T : class, IDataStore
        {
            return GetLocked(() => _Instance.GetDataStore<T>(consumer));
        }

        public void DoUnitOfWork<T>(Action<IDataStore> work) where T : class, IDataStore
        {
            DoLocked(() => _Instance.DoUnitOfWork<T>(work));
        }

        public TResult DoUnitOfWork<T, TResult>(Func<IDataStore, TResult> work)
            where T : class, IDataStore
        {
            return GetLocked(() => { return _Instance.DoUnitOfWork<T, TResult>(work); });
        }

        public void ReleaseDataStore<T>(object consumer) where T : class, IDataStore
        {
            DoLocked(() => _Instance.ReleaseDataStore<T>(consumer)); 
        }

        public void CloseDataStore<T>(object consumer) where T : class, IDataStore
        {
            DoLocked(() => _Instance.CloseDataStore<T>(consumer)); 
        }

        private void DoLocked(Action action)
        {
            lock (_LockObject)
            {
                action();
            }
        }

        private T GetLocked<T>(Func<T> action)
        {
            lock (_LockObject)
            {
                return action();
            }
        }

        public void CrossLoad<T>(T entity)
        {
            DoLocked(() => _Instance.CrossLoad(entity));
        }

        public void CrossLoad<T>(IEnumerable<T> entities)
        {
            DoLocked(() => _Instance.CrossLoad(entities));
        }
    }
}
