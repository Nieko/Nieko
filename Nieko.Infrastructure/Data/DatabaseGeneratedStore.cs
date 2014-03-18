using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Adapts a database-first EntityContext to implement IDataStore
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class DatabaseGeneratedStore<TContext> : IDataStore
        where TContext : ObjectContext
    {
        private static Dictionary<Type, DataStoreMethodsFinder> _DataStoreMethodsByType;

        private DataStoreMethodsFinder _Methods;
        private TContext _Context;
        private HashSet<StoredTypeInfo> _StoredTypes;

        protected virtual bool NoTracking
        {
            get
            {
                return false; 
            }
        }

        public ISet<StoredTypeInfo> StoredTypes
        {
            get
            {
                if (_StoredTypes == null)
                {
                    _StoredTypes = new HashSet<StoredTypeInfo>(
                        _Methods.EntityCollectionsByType
                            .Select(c => new StoredTypeInfo(c.Key, () => (IQueryable)c.Value)));
                }

                return _StoredTypes;
            }
        }

        public DatabaseGeneratedStore()
        {
            _Context = CreateContext();
            CheckStoreMethodsPopulated();
            _Methods = _DataStoreMethodsByType[_Context.GetType()];

            if (NoTracking)
            {
                var mergeOptionUpdater = new GeneratedStoreMergeOptionUpdater(_Methods);
                mergeOptionUpdater.SetMergeOption(_Context, MergeOption.NoTracking);  
            }
        }

        static DatabaseGeneratedStore()
        {
            _DataStoreMethodsByType = new Dictionary<Type, DataStoreMethodsFinder>();
        }

        public T GetItem<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            return GetTypeCollection<T>().FirstOrDefault(filter);  
        }

        public IQueryable<T> GetItems<T>()
        {
            return GetTypeCollection<T>();  
        }

        public void Save<T>(T item)
        {
            if ((item as EntityObject).EntityState == System.Data.EntityState.Added)
            {
                GetAddMethod<T>()(item);
                _Context.SaveChanges();
            }
        }

        public void Delete<T>(T item)
        {
            _Context.DeleteObject(item);
            _Context.SaveChanges(); 
        }

        public void Refresh(System.Collections.IEnumerable collection)
        {
            _Context.Refresh(RefreshMode.StoreWins, collection);
        }

        public void Refresh(object entity)
        {
            _Context.Refresh(RefreshMode.StoreWins, entity);
        }

        protected abstract TContext CreateContext();

        private ObjectQuery<T> GetTypeCollection<T>()
        {
            var accessor = (Func<TContext, ObjectQuery<T>>)_Methods.EntityCollectionsByType[typeof(T)];

            return accessor(_Context);
        }

        private Action<T> GetAddMethod<T>()
        {
            var addMethod = (Action<TContext, T>)_Methods.AddMethodsByType[typeof(T)];

            return o => addMethod(_Context, o);
        }

        private void CheckStoreMethodsPopulated()
        {
            lock (_DataStoreMethodsByType)
            {
                if (!_DataStoreMethodsByType.ContainsKey(_Context.GetType()))
                {
                    var methods = new DataStoreMethodsFinder(_Context.GetType());
                    _DataStoreMethodsByType.Add(_Context.GetType(), methods);
                }
            }
        }
    }
}