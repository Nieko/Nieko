using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Nieko.Infrastructure.Data
{
    internal class CrossContextLoader
    {
        private IDataStoresManager _DataStoresManager;
        private Dictionary<Type, object> _LoadersByEntityType = new Dictionary<Type, object>();

        public CrossContextLoader(IDataStoresManager dataStoresManager)
        {
            _DataStoresManager = dataStoresManager;
        }

        public void CrossLoad<T>(T entity)
        {
            foreach (var action in GetLoadActions<T>())
            {
                action(entity);
            }
        }

        public void CrossLoad<T>(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                CrossLoad(entity);
            }
        }

        private List<Action<T>> GetLoadActions<T>()
        {
            object actions;
            List<Action<T>> loadActions;

            if (!_LoadersByEntityType.TryGetValue(typeof(T), out actions))
            {
                loadActions = BuildLoadActions<T>();
                _LoadersByEntityType.Add(typeof(T), loadActions);
            }
            else
            {
                loadActions = (List<Action<T>>)actions;
            }

            return loadActions;
        }

        private List<Action<T>> BuildLoadActions<T>()
        {
            List<Action<T>> loadActions = new List<Action<T>>();
            List<Action<IDataStore, T>> dataStoreUnitOfWork;
            Action<IDataStore, T> setAction;
            Action<Action<IDataStore>> unitOfWorkAction;

            MethodInfo buildMethod = typeof(CrossContextLoader).GetMethod("BuildSetter", BindingFlags.Instance | BindingFlags.NonPublic); 

            var crossProperties = typeof(T).GetPropertiesWithAttribute<CrossContextAttribute>()
                .GroupBy(cp => (Attribute.GetCustomAttribute(cp, typeof(CrossContextAttribute)) as CrossContextAttribute).DataStore);

            foreach (var dataStoreProperties in crossProperties)
            {
                dataStoreUnitOfWork = new List<Action<IDataStore, T>>();

                foreach (var crossReference in dataStoreProperties)
                {
                    setAction = (Action<IDataStore, T>)buildMethod.MakeGenericMethod(typeof(T), crossReference.PropertyType).Invoke(this, new object[]{crossReference});

                    dataStoreUnitOfWork.Add(setAction);
                }

                unitOfWorkAction = GetUnitOfWorkFunction(dataStoreProperties.Key);

                loadActions.Add(o =>
                    {
                        unitOfWorkAction(dataStore =>
                            {
                                foreach (var action in dataStoreUnitOfWork)
                                {
                                    action(dataStore, o);
                                }
                            });
                    });
            }

            return loadActions;
        }

        private Action<IDataStore, T> BuildSetter<T, TEntity>(PropertyInfo property)
            where T : IPrimaryKeyed
        {
            var objectParameter = Expression.Parameter(typeof(T), "o");
            var entityParameter = Expression.Parameter(typeof(TEntity), "e");

            Action<T, TEntity> setter = Expression.Lambda<Action<T, TEntity>>(
                Expression.Call(
                    objectParameter,
                    property.GetSetMethod(),
                    entityParameter),
                objectParameter,
                entityParameter).Compile();

            Action<IDataStore, T> retrieveAndSet = (d, o) =>
                {
                    var entity = d.GetItem<TEntity>(o.PrimaryKey.ToFilterExpression<TEntity>());
                    setter(o, entity);
                };

            return retrieveAndSet;
        }

        private Action<Action<IDataStore>> GetUnitOfWorkFunction(Type dataStoreType)
        {
            var unitOfWorkParameter = Expression.Parameter(typeof(Action<IDataStore>), "work");

            return Expression.Lambda<Action<Action<IDataStore>>>(
                Expression.Call(
                    dataStoreType.GetMethod("GetDataStore").MakeGenericMethod(dataStoreType),
                    unitOfWorkParameter),
                unitOfWorkParameter)
                .Compile();
        }
    }
}
