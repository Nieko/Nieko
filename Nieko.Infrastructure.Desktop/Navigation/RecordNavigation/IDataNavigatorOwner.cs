using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Specialized;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Responsible for co-ordinating a data tier
    /// </summary>
    /// <remarks>
    /// Holds and coordinates Navigation and Persistence components for a tier of the graph and 
    /// relationships between parent and child Navigation Owners 
    /// </remarks>
    public interface ITierCoordinator : INotifyDisposing, INotifyPropertyChanged 
    {
        /// <summary>
        /// Raised when the collection of Model View items at this level changes
        /// </summary>
        event NotifyCollectionChangedEventHandler ItemsCollectionChanged;
        /// <summary>
        /// Raised when this tier has persisted changes and expects child
        /// Navigation Owners to begin persisting their changes
        /// </summary>
        event EventHandler<PersistingChangesEventArgs> PersistingChanges;
        /// <summary>
        /// Indicates whether, given the navigation position in the data,
        /// edits should be allowed
        /// </summary>
        bool AllowEdit { get; }
        /// <summary>
        /// Returns the common type (if any) of items in the <seealso cref="PersistedView"/>
        /// </summary>
        Type ItemType { get; }
        /// <summary>
        /// Persistence for this tier
        /// </summary>
        IPersistedView PersistedView { get; }
        /// <summary>
        /// Navigation actions
        /// </summary>
        IDataNavigator DataNavigator { get; }
        /// <summary>
        /// Owner of the next higher tier
        /// </summary>
        ITierCoordinator Parent { get; }
        /// <summary>
        /// Parent - Child hierarchy for this level
        /// </summary>
        IOwnershipHierarchy Hierarchy { get; }
    }
}
