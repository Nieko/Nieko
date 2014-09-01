using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure;
using System.Data.Entity;
using System.Collections;
using System.Linq.Expressions;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.EntityFramework
{
    public abstract class CodeFirstStore: DbContext, IDataStore
    {
        private static Dictionary<Type, ICodeFirstTypeStoreMethods> _StoreMethodsByType = new Dictionary<Type, ICodeFirstTypeStoreMethods>();
        private static Dictionary<Type, ISet<StoredTypeInfo>> _StoredTypeInfoByContextType = new Dictionary<Type, ISet<StoredTypeInfo>>();

        public virtual void Delete<T>(T item)
        {
            GetStoreMethods<T>().Delete(this, item);
        }

        public virtual T GetItem<T>(Expression<Func<T, bool>> filter)
        {
            return GetStoreMethods<T>().GetItem(this, filter); 
        }

        public virtual ISet<StoredTypeInfo> StoredTypes
        {
            get 
            {
                ISet<StoredTypeInfo> storedTypes = null;

                if (!_StoredTypeInfoByContextType.TryGetValue(GetType(), out storedTypes))
                {
                    storedTypes = new HashSet<StoredTypeInfo>(
                        GetType()
                        .GetProperties()
                        .Where(p => typeof(DbSet<>).IsAssignableFrom(p.PropertyType))
                        .Select(p => new StoredTypeInfo(p.PropertyType.GetGenericArguments()[0],
                            () => (IQueryable)Set(p.PropertyType.GetGenericArguments()[0]))));

                    _StoredTypeInfoByContextType.Add(GetType(), storedTypes);
                }

                return storedTypes;
            }
        }

        public virtual IQueryable<T> GetItems<T>()
        {
            return GetStoreMethods<T>().GetItems<T>(this); 
        }

        public virtual void Save<T>(T item)
        {
            GetStoreMethods<T>().Save(this, item);
        }

        public virtual void Refresh(IEnumerable collection)
        {
            foreach (var item in collection.Cast<object>())
            {
                Refresh(item);
            }
        }

        public virtual void Refresh(object entity)
        {
            Entry(entity).Reload();  
        }

        private static ICodeFirstTypeStoreMethods GetStoreMethods<T>()
        {
            ICodeFirstTypeStoreMethods methods = null;

            if (!_StoreMethodsByType.TryGetValue(typeof(T), out methods))
            {
                var storeMethodsType = typeof(CodeFirstTypeStoreMethods<,>).MakeGenericType(typeof(T), typeof(T));
                methods = (ICodeFirstTypeStoreMethods)Activator.CreateInstance(storeMethodsType);

                _StoreMethodsByType.Add(typeof(T), methods);
            }

            return methods;
        }
    }
}
