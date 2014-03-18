using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Objects;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Finds EntityContext members and details information
    /// regarding Entity Collection Properties and Entity Add Methods
    /// </summary>
    public class DataStoreMethodsFinder
    {
        private Type _DataStoreType;

        public Dictionary<Type, object> EntityCollectionsByType { get; private set; }
        public Dictionary<Type, object> AddMethodsByType { get; private set; }

        public DataStoreMethodsFinder(Type dataStoreType)
        {
            _DataStoreType = dataStoreType;

            PopulateEntityCollectionsByType();
            PopulateAddMethodsByType();
        }

        private void PopulateEntityCollectionsByType()
        {
            EntityCollectionsByType = new Dictionary<Type, object>();

            var entityCollections = _DataStoreType.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ObjectSet<>));
            Type itemType;
            MethodInfo accessorBuilder = typeof(DataStoreMethodsFinder).GetMethod("BuildCollectionAccessor", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var entityCollection in entityCollections)
            {
                itemType = entityCollection.PropertyType.GetGenericArguments()[0];
                EntityCollectionsByType.Add(itemType, accessorBuilder.MakeGenericMethod(_DataStoreType, itemType).Invoke(this, new object[] { entityCollection }));
            }
        }

        private Func<TDataStore, ObjectQuery<T>> BuildCollectionAccessor<TDataStore, T>(PropertyInfo property)
        {
            var containerParameter = Expression.Parameter(typeof(TDataStore), "c");

            var accessor = Expression.Lambda<Func<TDataStore, ObjectQuery<T>>>(
                Expression.PropertyOrField(
                    containerParameter,
                    property.Name),
                containerParameter);

            return accessor.Compile();
        }

        private void PopulateAddMethodsByType()
        {
            AddMethodsByType = new Dictionary<Type, object>();

            var addMethods = _DataStoreType.GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                .Where(m => m.Name.Length > 5 &&
                    m.Name.StartsWith("AddTo") &&
                    m.GetParameters().Count() == 1);
            Type entityType;
            MethodInfo actionBuilder = typeof(DataStoreMethodsFinder).GetMethod("BuildAddAction", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach(var addMethod in addMethods)
            {
                entityType = addMethod.GetParameters()[0].ParameterType;

                AddMethodsByType.Add(entityType, actionBuilder.MakeGenericMethod(_DataStoreType, entityType).Invoke(this, new object[]{ addMethod }));  
            }
 
        }

        private Action<TDataStore, T> BuildAddAction<TDataStore, T>(MethodInfo method)
        {
            var containerParameter = Expression.Parameter(typeof(TDataStore), "c");
            var entityParameter = Expression.Parameter(typeof(T), "o");

            var addAction = Expression.Lambda<Action<TDataStore, T>>(
                Expression.Call(
                    containerParameter,
                    method,
                    entityParameter),
                containerParameter,
                entityParameter);

            return addAction.Compile(); 
        }
    }
}
