using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Default IModelViewStoresManager implementation that searches for all
    /// IModelViewStoreRegistration implementing concrete classes at start-up and
    /// includes them for management
    /// </summary>
    public class PluginModelViewStoreManager : IModelViewStoresManager
    {
        private static DescendedDateComparer _UsedFromComparer = new DescendedDateComparer();
        private Dictionary<Type, SortedList<DateTime, Func<DateTime, IModelViewStore>>> _ModelViewStores;
        private Action _InitializeModelViewStores;

        protected Dictionary<Type, SortedList<DateTime, Func<DateTime, IModelViewStore>>> ModelViewStores
        {
            get
            {
                if (_ModelViewStores == null)
                {
                    _InitializeModelViewStores();
                }
                return _ModelViewStores;
            }
        }

        public T GetModelViewStore<T>(DateTime ruleSetDate) where T : IModelViewStore
        {
            if (!ModelViewStores.ContainsKey(typeof(T)))
            {
                throw new KeyNotFoundException("ModelViewStoreManager does not manage ViewModelStores of type " + typeof(T));
            }

            var stores = ModelViewStores[typeof(T)];

            return (T)stores.First(kvp => kvp.Key <= ruleSetDate).Value(ruleSetDate);
        }

        public T GetModelViewStore<T>() where T : IModelViewStore
        {
            return GetModelViewStore<T>(DateTime.MinValue); 
        }

        public PluginModelViewStoreManager(Func<Type, IModelViewStore> modelViewStoreFactory, IPluginFinder pluginFinder)
        {
            pluginFinder.RegisterCreatePluginsCallBack<IModelViewStoreRegistration>(plugIns =>
                {
                    ProcessRegistrations(modelViewStoreFactory, plugIns);
                });
        }

        public void DoUnitOfWork<T>(DateTime rulesSetDate, Action<T> work) where T : IModelViewStore
        {
            lock (ModelViewStores)
            {
                work(GetModelViewStore<T>(rulesSetDate)); 
            }
        }

        public void DoUnitOfWork<T>(Action<T> work) where T : IModelViewStore
        {
            DoUnitOfWork(DateTime.MinValue, work); 
        }

        private void ProcessRegistrations(Func<Type, IModelViewStore> modelViewStoreFactory, IEnumerable<IModelViewStoreRegistration> registrations)
        {
            _InitializeModelViewStores = () =>
                {
                    SortedList<DateTime, Func<DateTime, IModelViewStore>> viewStoresByUseDate = null;

                    _ModelViewStores = new Dictionary<Type, SortedList<DateTime, Func<DateTime, IModelViewStore>>>();

                    foreach (var item in registrations)
                    {
                        if (!ModelViewStores.TryGetValue(item.BaseStoreType, out viewStoresByUseDate))
                        {
                            viewStoresByUseDate = new SortedList<DateTime, Func<DateTime, IModelViewStore>>(_UsedFromComparer);
                            ModelViewStores.Add(item.BaseStoreType, viewStoresByUseDate);
                        }

                        var registration = item;

                        viewStoresByUseDate.Add(registration.UsedFrom, asAt =>
                            {
                                var store = modelViewStoreFactory(registration.Type);

                                registration.SetDateAsAt(store, asAt);

                                return store;
                            });
                    }

                    _InitializeModelViewStores = null;
                };
        }
    }
}
