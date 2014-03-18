using System;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Most top-level node / tier in a ModelView graph
    /// </summary>
    public interface IPersistedViewRoot : IPersistedView<IPersistedViewRoot>, IEditableMirrorObject
    {
        /// <summary>
        /// Manager for persistence side of ModelView graph
        /// </summary>
        IDataStoresManager Manager { get; }
        /// <summary>
        /// Manager for presentation side of ModelView graph
        /// </summary>
        IViewNavigator RegionNavigator { get; }
        /// <summary>
        /// Lifetime provider for graph
        /// </summary>
        INotifyDisposing Root { get; set; }
        /// <summary>
        /// Initial load of items from data store once graph is constructed
        /// </summary>
        void Load();
        /// <summary>
        /// Retrieve current items into graph
        /// </summary>
        void Reload();
    }
}
