using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.ComponentModel;
using System.ComponentModel;
using System.Collections;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Persistence for a ModelView collection view
    /// </summary>
    public interface IPersistedView : INotifyDisposing, INotifyPropertyChanged, INotifyModelViewGraphNodeLoaded, ICollectionViewWrapper
    {
        /// <summary>
        /// Persisting Events
        /// </summary>
        event EventHandler<PersistedViewPersistingEventArgs> Persisting;
        /// <summary>
        /// Raised when Collection View component(s) are available or changed
        /// </summary>
        event EventHandler ViewChanged;
        /// <summary>
        /// Coordinator of data navigation for collection view
        /// </summary>
        ITierCoordinator Owner { get; }
        /// <summary>
        /// Dictionary of all unsaved items marked for deletion by their PrimaryKey
        /// </summary>
        ReadOnlyDictionary<PrimaryKey> DeletedItems { get; }
        /// <summary>
        /// Wrapper of <see cref="View.CurrentPosition"/> implementing Property Change
        /// notifications
        /// </summary>
        new int CurrentPosition { get; set; }
        /// <summary>
        /// Persist edits and deletions of item(s) within <see cref="View"/>
        /// </summary>
        /// <param name="parent">Persistence for parent tier / leaf in graph</param>
        void PersistChanges(IPersistedView parent);
        /// <summary>
        /// Executes the action, ensuring that persistence data store is open for the duration
        /// </summary>
        /// <param name="storeOpenedAction">Store action</param>
        void RunOpened(Action storeOpenedAction);
        /// <summary>
        /// Changes the source collection
        /// </summary>
        void SetSource(IList items);
    }
}
