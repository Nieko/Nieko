using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Data
{
    public abstract class DataStoreRegistration<T> : IDataStoreRegistration
        where T : IDataStore
    {
        private bool _ConnectionNameSet = false;
        private string _ConnectionDetails = string.Empty;
        private ConnectionDetailsFormat _ConnectionType;

        public static string DefaultConnectionStringName { get { return "DataStore";}}

        public Type Type { get { return typeof(T); } }

        public virtual ConnectionDetailsFormat ConnectionType
        {
            get
            {
                if (!_ConnectionNameSet)
                {
                    InitializeConnectionProperties();
                }
                return _ConnectionType;
            }
            protected set
            {
                _ConnectionType = value;
            }
        }

        public string ConnectionDetails
        {
            get
            {
                if(!_ConnectionNameSet)
                {
                    InitializeConnectionProperties();
                }

                return _ConnectionDetails;
            }
            protected set
            {
                _ConnectionDetails = value;
            }
        }

        protected abstract string GetConnectionDetails();

        protected virtual void ConnectionDetailsSet() { }
       
        private void InitializeConnectionProperties()
        {
            _ConnectionDetails = GetConnectionDetails();
            ConnectionType = ConnectionDetailsFormat.ConfigName;
            ConnectionDetailsSet();
        }
    }
}
