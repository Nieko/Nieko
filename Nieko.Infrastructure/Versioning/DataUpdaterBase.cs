using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Versioning;

namespace Nieko.Infrastructure.Versioning
{
    /// <summary>
    /// Base class providing general implementation of a IDataUpdater
    /// </summary>
    /// <remarks>
    /// Relies on the implementation of the various other versioning interfaces to 
    /// provide specific functionality
    /// </remarks>
    public abstract class DataUpdaterBase : IDataUpdater
    {
        private IRoutinesProvider _RoutinesProvider;
        private IVersionPersistenceProvider _VersionPersistenceProvider;
        private SortedList<Version, Version> _Versions = new SortedList<Version, Version>();

        public IVersionedComponent Component { get; private set;}

        public IDictionary<Version, IList<IVersionUpdaterRoutine>> UpdateRoutinesByVersion { get; private set; }

        public SortedList<Version, Version> Versions
        {
            get
            {
                return _Versions;
            }
        }

        public Version GetDataVersion()
        {
            return _VersionPersistenceProvider.GetDataVersion();
        }

        /// <summary>
        /// Name of component that this updater is for
        /// </summary>
        /// <remarks>
        /// Is used by the Component Supplier constructor argument to retrieve
        /// the actual component implementation
        /// </remarks>
        protected abstract string ComponentName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentSupplier">Retrieval of component by name</param>
        /// <param name="persistenceProviderSupplier">Retrieval of version persistence by name</param>
        /// <param name="routinesProvider">Source of all routines</param>
        public DataUpdaterBase(Func<string, IVersionedComponent> componentSupplier, Func<string, IVersionPersistenceProvider> persistenceProviderSupplier, IRoutinesProvider routinesProvider)
        {
            _RoutinesProvider = routinesProvider;
            Component = componentSupplier(ComponentName); 
            UpdateRoutinesByVersion = _RoutinesProvider.CreateComponentRoutines(Component);
            _VersionPersistenceProvider = persistenceProviderSupplier(Component.Name);

            foreach (var version in UpdateRoutinesByVersion.Keys)
            {
                Versions.Add(version, version);
            }
        }

        public void SaveVersion(Version version)
        {
            _VersionPersistenceProvider.SaveVersion(version);
        }

        public void UpdateData(Version toVersion)
        {
            var processedRoutines = new HashSet<Type>();
            List<IVersionUpdaterRoutine> routinesRemaining;
            int lastCount;
            Version dataVersion;

            dataVersion = GetDataVersion();

            foreach (var version in Versions.Values.Where(v => v.CompareTo(dataVersion) == 1 && v.CompareTo(toVersion) > -1))
            {
                routinesRemaining = new List<IVersionUpdaterRoutine>(UpdateRoutinesByVersion[version]);
                while (routinesRemaining.Count > 0)
                {
                    lastCount = routinesRemaining.Count;
                    foreach (var routine in new List<IVersionUpdaterRoutine>(routinesRemaining))
                    {
                        if (routine.DependsOn.Any(depends => !processedRoutines.Contains(depends)))
                        {
                            continue;
                        }

                        routine.ApplyDataUpdate();
                        routinesRemaining.Remove(routine);
                        processedRoutines.Add(routine.GetType());
                    }
                    if (lastCount == routinesRemaining.Count)
                    {
                        throw new VersioningException("Cannot resolve Circular Update Routine dependencies");
                    }
                }
                SaveVersion(version);
            }
        }
    }
}