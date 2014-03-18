using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using System.Configuration;
using Nieko.Infrastructure.Reflection;

namespace Nieko.Modules.Architecture
{
    public class StoreConnectionStringProvider : IStoreConnectionStringProvider
    {
        private Dictionary<Type, string> _StoreConnectionNames;

        public string this[Type storeType]
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[_StoreConnectionNames[storeType]].ConnectionString;
            }
        }

        public StoreConnectionStringProvider(IPluginFinder pluginFinder)
        {
            pluginFinder.RegisterCreatePluginsCallBack<IDataStoreRegistration>(registrations =>
                {
                    _StoreConnectionNames = registrations.ToDictionary(r => r.Type, r => r.ConnectionStringName);
                });
        }
    }
}
