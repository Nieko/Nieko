using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.ComponentModel;
using System.ComponentModel;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Generic functionality for a <seealso cref="IPersistedView"/>
    /// </summary>
    /// <typeparam name="T">ModelView</typeparam>
    public interface IPersistedView<T> : IPersistedView
            where T : IEditableMirrorObject
    {
        /// <summary>
        /// Source collection for <seealso cref="IPersistedView.View"/>
        /// </summary>
        ObservableCollection<T> Items { get; set; }
        /// <summary>
        /// Type safe wrapper for <seealso cref="IPersistedView.View"/> CurrentItem that implements
        /// Property changed notification
        /// </summary>
        new T CurrentItem { get; }
        /// <summary>
        /// Method for refreshing items from data store
        /// </summary>
        /// <returns>Current items</returns>
        IEnumerable<T> ItemsLoader();
    }
}