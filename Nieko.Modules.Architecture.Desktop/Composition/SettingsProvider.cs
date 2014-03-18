using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Composition;

namespace Nieko.Modules.Architecture.Composition
{
    public class SettingsProvider : ISettingsProvider
    {
        private IGenericSupplierBuilder _FactoryBuilder;
        private Dictionary<Type, ISettings> _SettingsByType = new Dictionary<Type,ISettings>();

        public SettingsProvider(IGenericSupplierBuilder factoryBuilder)
        {
            _FactoryBuilder = factoryBuilder;
        }

        public T GetSettings<T>() where T : class, ISettings
        {
            ISettings settings;

            if (!_SettingsByType.TryGetValue(typeof(T), out settings))
            {
                settings = _FactoryBuilder.BuildSupplier<T>()();
                _SettingsByType[typeof(T)] = settings;
                settings.Refresh(); 
            }

            return (T)settings;
        }
    }
}
