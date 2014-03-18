using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Composition;

namespace Nieko.Infrastructure.Data
{
    /// <summary>
    /// Default IDataStoresManager implementation that searches for all
    /// IDataStoreRegistration implementing concrete classes at start-up and
    /// includes them for management
    /// </summary>
    public class PluginDataStoresManager : SynchronizedDataStoresManager
    {
        public PluginDataStoresManager(IPluginFinder plugInFinder, Func<Type, IDataStore> storeSupplier, Func<IModelViewStoresManager> modelViewStoresManagerFactory)
            : base(storeSupplier, modelViewStoresManagerFactory)
        {
            plugInFinder.RegisterCreatePluginsCallBack<IDataStoreRegistration>(
                (instances) => 
                    {
                        DataStoreTypes = new List<Type>(instances.Select(plugIn => plugIn.Type));
                    });
        }
    }
}
