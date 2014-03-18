using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Versioning;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// <seealso cref="IVersionPersistenceProvider"/> implementation backed by a Data Store
    /// </summary>
    /// <typeparam name="TDataStore">Data store for versioning details</typeparam>
    /// <typeparam name="TVersion">Entity for version data</typeparam>
    /// <typeparam name="TComponent">Entity for component data</typeparam>
    public abstract class VersionPersistenceProvider<TDataStore, TVersion, TComponent> : IVersionPersistenceProvider
        where TDataStore : class, IDataStore
        where TVersion : class, IVersionEntity, new()
        where TComponent : class, IComponentEntity, new()
    {
        IDataStoresManager _DataStoresManager;

        public VersionPersistenceProvider(IDataStoresManager dataStoresManager)
        {
            _DataStoresManager = dataStoresManager;
        }

        protected virtual Expression<Func<TVersion, bool>> VersionEntityFilter
        {
            get
            {
                return ve => true;
            }
        }

        protected virtual Action<IDataStore, TVersion> VersionInitalisation
        {
            get
            {
                return (dsm, ve) => { return; };
            }
        }

        public System.Version GetDataVersion()
        {
            System.Version dataVersion = null;

            _DataStoresManager.DoUnitOfWork<TDataStore>((dataStore) =>
            {
                var version = dataStore.GetItems<TVersion>()
                    .Where(VersionEntityFilter)
                    .OrderBy(v => new { v.Major, v.Minor, v.Build, v.Revision }).FirstOrDefault();

                if (version == null)
                {
                    dataVersion = new System.Version(0, 0, 0, 0);
                }
                else
                {
                    dataVersion = new System.Version(version.Major, version.Minor, version.Build, version.Revision);
                }
            });

            return dataVersion;
        }

        public void SaveVersion(System.Version version)
        {
            _DataStoresManager.DoUnitOfWork<TDataStore>((dataStore) =>
            {
                var versionEntity = new TVersion();
                versionEntity.Build = version.Build;
                versionEntity.Major = version.Major;
                versionEntity.Minor = version.Minor;
                versionEntity.Revision = version.Revision;
                versionEntity.Upgraded = DateTime.Now;
                VersionInitalisation(dataStore, versionEntity);

                dataStore.Save<TVersion>(versionEntity);
            });
        }

        protected TComponent GetComponent(IDataStore dataStore, string componentName)
        {
            TComponent projectComponent = null;

            projectComponent = dataStore.GetItem<TComponent>(c => c.Name == componentName);
            if (projectComponent == null)
            {
                projectComponent = new TComponent();
                projectComponent.Name = componentName;
                dataStore.Save(projectComponent);
            }

            return projectComponent;
        }

        protected Expression<Func<TVersion, bool>> GetVersionFilter(string componentName)
        {
            return (TVersion v) => v.Component.Name == componentName;
        }

    }
}