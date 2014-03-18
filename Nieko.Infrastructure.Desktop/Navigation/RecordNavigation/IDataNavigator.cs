using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Controls;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Provides the ability to navigate through and edit specific instances 
    /// of a <seealso cref="ListCollectionView"/> of editable objects
    /// </summary>
    public interface IDataNavigator : INotifyPropertyChanged, INotifyDisposing
    {
        /// <summary>
        /// Occurs when the current item of the ListCollectionView has changed
        /// </summary>
        event EventHandler ViewCurrentChanged;
        /// <summary>
        /// Occurs when the current item of the ListCollectionView is about to
        /// change
        /// </summary>
        event EventHandler<CurrentChangingEventArgs> ViewCurrentChanging;
        /// <summary>
        /// Returns whether or not this Data Navigator provides a UI Control
        /// </summary>
        bool HasView { get; }
        /// <summary>
        /// Number of items in ListCollectionView
        /// </summary>
        int Count { get; set; }
        /// <summary>
        /// Ordinal position of the current item of ListCollectionView
        /// </summary>
        int CurrentPosition { get; set; }
        /// <summary>
        /// Initialization method for new items in ListCollectionView 
        /// </summary>
        Action<object> Creator { get; set; }
        /// <summary>
        /// Tier owner
        /// </summary>
        IDataNavigatorOwner Owner { get; set; }
        /// <summary>
        /// Current edit state
        /// </summary>
        EditState EditState { get; }
        /// <summary>
        /// Navigates through ListCollectionView, changing the current
        /// item to that at the indicated position
        /// </summary>
        /// <param name="navigation">Navigation to perform</param>
        void Navigate(RecordNavigation navigation);
        /// <summary>
        /// Navigates to a specific position in the ListCollectionView, 
        /// changing the current item to that of the indicated position
        /// </summary>
        /// <param name="position">Index of item to navigate to</param>
        void NavigateTo(int position);
        /// <summary>
        /// Perform an edit action
        /// </summary>
        /// <param name="state">Edit action to perform</param>
        void EnterState(EditState state);
        /// <summary>
        /// Finds the first item of type <typeparamref name="T"/> that
        /// satisfies the filter condition
        /// </summary>
        /// <typeparam name="T">Type of item to search for</typeparam>
        /// <param name="filter">Search filter</param>
        /// <returns></returns>
        bool Find<T>(Func<T, bool> filter)
            where T : class;
    }
}
