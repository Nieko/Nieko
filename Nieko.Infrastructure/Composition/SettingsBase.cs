using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// IDataStore backed settings class
    /// </summary>
    /// <typeparam name="TStore">Store in which settings reposes</typeparam>
    /// <typeparam name="TSettingsEntity">Individual Setting persistence implementation</typeparam>
    /// <remarks>
    /// Implementations are accessed via ISettingsProvider e.g.
    /// <c>var windowSettings = settingsProvider.GetSettings&lt;WindowSettings&gt;();</c>
    /// </remarks>
    public abstract class SettingsBase<TStore, TSettingsEntity> : ISettings
        where TStore : class, IDataStore
        where TSettingsEntity : ISettingsEntity
    {
        private object lockDummy = new object();
        private IDataStoresManager _DataStoresManager;
        private List<Action<object, Dictionary<string, TSettingsEntity>>> _Persister;
        private List<Action<object, Dictionary<string, TSettingsEntity>>> _Refresher;

        protected abstract IList<TSettingsEntity> LoadEntities(IDataStore dataStore);
        protected abstract TSettingsEntity CreateEntity();

        public SettingsBase(IDataStoresManager dataStoresManager)
        {
            _DataStoresManager = dataStoresManager;
        }

        public virtual void Save()
        {
            lock (_Persister)
            {
                if (_Persister == null)
                {
                    BuildAccessors();
                }

                _DataStoresManager.DoUnitOfWork<TStore>(dataStore =>
                    {
                        var settings = LoadEntities(dataStore).
                            ToDictionary(s => s.Name);

                        foreach (var persist in _Persister)
                        {
                            persist(this, settings);
                        }

                        foreach (var setting in settings.Values)
                        {
                            dataStore.Save(setting);
                        }
                    });
            }
        }

        public virtual void Refresh()
        {
            lock (lockDummy)
            {
                if (_Persister == null)
                {
                    BuildAccessors();
                }

                _DataStoresManager.DoUnitOfWork<TStore>(dataStore =>
                {
                    var settings = LoadEntities(dataStore).
                        ToDictionary(s => s.Name);

                    foreach (var refresh in _Refresher)
                    {
                        refresh(this, settings);
                    }
                });
            }
        }

        private void BuildAccessors()
        {
            var instanceParameter = Expression.Parameter(typeof(object), "o");
            var entitiesParameter = Expression.Parameter(typeof(Dictionary<string, TSettingsEntity>), "settings");

            var properties = GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            _Persister = properties.Select(
                p =>
                {
                    Action<object, Dictionary<string, TSettingsEntity>> persist =
                        (object o, Dictionary<string, TSettingsEntity> settings) =>
                        {
                            TSettingsEntity setting;
                            if (!settings.TryGetValue(p.Name, out setting))
                            {
                                setting = CreateEntity();
                                setting.Name = p.Name;
                                settings.Add(p.Name, setting);
                            }

                            setting.Value = (string)p.GetValue(o, null);
                        };

                    return persist;
                })
                    .ToList();

            _Refresher = properties.Select(
                p =>
                {
                    Action<object, Dictionary<string, TSettingsEntity>> refresh =
                        (object o, Dictionary<string, TSettingsEntity> settings) =>
                        {
                            TSettingsEntity setting;
                            if (!settings.TryGetValue(p.Name, out setting))
                            {
                                return;
                            }

                            p.SetValue(o, setting.Value, null); 
                        };

                    return refresh;
                })
                    .ToList();

        }
    }
}
