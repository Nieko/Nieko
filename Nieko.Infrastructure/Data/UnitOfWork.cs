using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Wrapper IDataStore implementation that wraps a Data Store
    /// and lets an external object IDataStoresManager manage its
    /// lifetime.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private IDataStoresManager _DataStoresManager;
        private Action<UnitOfWork> _StoreRelease;
        private IDataStore _TargetStore;

        public ISet<StoredTypeInfo> StoredTypes
        {
            get { return _TargetStore.StoredTypes; }
        }

        public UnitOfWork(IDataStoresManager dataStoresManager)
        {
            _DataStoresManager = dataStoresManager;
        }

        public void Begin<T>()
            where T : class, IDataStore
        {
            _TargetStore = _DataStoresManager.GetDataStore<T>(this);
            _StoreRelease = (owner) =>
                {
                    var dataStoresReleaser = _DataStoresManager;
                    dataStoresReleaser.ReleaseDataStore<T>(owner);
                };
        }

        public void Delete<T>(T item)
        {
            _TargetStore.Delete(item);
        }

        public void Refresh(System.Collections.IEnumerable collection)
        {
            _TargetStore.Refresh(collection);
        }

        public void Refresh(object entity)
        {
            _TargetStore.Refresh(entity);
        }

        public void Dispose()
        {
            if (_StoreRelease != null)
            {
                _StoreRelease(this);
            }
        }

        public T GetItem<T>(Expression<Func<T, bool>> filter)
        {
            return _TargetStore.GetItem<T>(filter);
        }

        public IQueryable<T> GetItems<T>()
        {
            return _TargetStore.GetItems<T>();
        }

        public void Save<T>(T item)
        {
            _TargetStore.Save<T>(item);
        }

        public void Save(object item)
        {
            _TargetStore.Save(item);
        }
    }
}