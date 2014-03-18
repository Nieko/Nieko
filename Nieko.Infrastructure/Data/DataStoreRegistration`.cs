using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    public abstract class DataStoreRegistration<T> : IDataStoreRegistration
        where T : IDataStore
    {
        public static string DefaultConnectionStringName { get { return "DataStore";}}

        public Type Type { get { return typeof(T); } }

        public string ConnectionStringName { get; protected set; }

        public DataStoreRegistration()
        {
            SetConnectionStringName();
        }

        protected abstract void SetConnectionStringName();
    }
}
