using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.EntityFramework
{
    public abstract class DesignCodeFirstStore<TStore> : IDataStore
        where TStore : CodeFirstStore
    {
        private Dictionary<Type, object> _TypeCollections;
        private ISet<StoredTypeInfo> _StoredTypes;

        public ISet<StoredTypeInfo> StoredTypes
        {
            get 
            {
                if (_TypeCollections == null)
                {
                    _TypeCollections = typeof(TStore).GetProperties()
                        .Where(p => typeof(IQueryable).IsAssignableFrom(p.PropertyType) &&
                            p.PropertyType.GetGenericArguments().Length == 1)
                        .Select(p =>
                        {
                            var arg = p.PropertyType.GetGenericArguments()[0];
                            var collection = Activator.CreateInstance(typeof(List<>).MakeGenericType(arg));

                            return new { Type = arg, Collection = collection };
                        })
                        .ToDictionary(p => p.Type, p => p.Collection);

                    _StoredTypes = new HashSet<StoredTypeInfo>(_TypeCollections
                        .Select(tc => new StoredTypeInfo(tc.Key, () => (tc.Value as System.Collections.IList).AsQueryable())));
                }

                return _StoredTypes;
            }
        }

        public IQueryable<T> GetItems<T>()
        {
            return (IQueryable<T>)(StoredTypes.First(f => f.ItemType == typeof(T))
                .ItemRetrieval());  
        }

        public void Delete<T>(T item)
        {
            if (StoredTypes != null)
            {
                (_TypeCollections[typeof(T)] as List<T>).Remove(item); 
            }
        }

        public T GetItem<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            return GetItems<T>()
                .FirstOrDefault(filter); 
        }

        public void Save<T>(T item)
        {
            if (StoredTypes != null)
            {
                (_TypeCollections[typeof(T)] as List<T>).Add(item);
            }
        }

        public void Refresh(System.Collections.IEnumerable collection) { }

        public void Refresh(object entity) { }
    }
}
