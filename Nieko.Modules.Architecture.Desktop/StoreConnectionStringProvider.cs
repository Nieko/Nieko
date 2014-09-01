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
        private string _DefaultConnectionString = string.Empty;
        private Dictionary<string, string> _StoreConnectionStrings;

        public string this[Type storeType]
        {
            get
            {
                if (_StoreConnectionStrings == null)
                {
                    _StoreConnectionStrings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
                        .ToDictionary(css => css.Name, css => css.ConnectionString);
                    if (ConfigurationManager.AppSettings.AllKeys.Any(k => k == "DefaultConnectionStore"))
                    {
                        _DefaultConnectionString = ConfigurationManager.AppSettings["DefaultConnectionStore"];
                    }
                }

                if (_StoreConnectionStrings.ContainsKey(_StoreConnectionNames[storeType]))
                {
                    return _StoreConnectionStrings[_StoreConnectionNames[storeType]];
                }

                if (_DefaultConnectionString == string.Empty)
                {
                    throw new System.Configuration.ConfigurationErrorsException("Cannot find connection string for Store " + storeType.Name + " and no DefaultConnectionStore found in AppSettings");
                }

                return _StoreConnectionStrings[_DefaultConnectionString];
            }
        }

        public StoreConnectionStringProvider(IPluginFinder pluginFinder)
        {
            pluginFinder.RegisterCreatePluginsCallBack<IDataStoreRegistration>(registrations =>
                {
                    _StoreConnectionNames = registrations.ToDictionary(r => r.Type, r => r.ConnectionDetails);
                });
        }
    }
}
