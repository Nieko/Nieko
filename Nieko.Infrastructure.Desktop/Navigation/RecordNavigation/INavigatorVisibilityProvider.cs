using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Visibility settings for an <seealso cref="IDataNavigatorViewModel"/> instance
    /// indicating which edit and navigation actions should be hidden
    /// </summary>
    /// <remarks>
    /// <seealso cref="INotifyPropertyChanged"/> is implemented for all members
    /// raising the event for <see cref="VisibleEditStates"/> and <see cref="VisibleNavigationStates"/>
    /// if their underlying collections change
    /// </remarks>
    public interface INavigatorVisibilityProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates all edit states may be shown
        /// </summary>
        bool ShowAllEditStates { get; }
        /// <summary>
        /// Indicates all navigation actions may be shown
        /// </summary>
        bool ShowAllNavigationStates { get; }
        /// <summary>
        /// Indicates whether or not to hide the entire navigation bar
        /// regardless of individual edit and/or navigation visibility
        /// </summary>
        bool ShowNavigationBar { get; }
        /// <summary>
        /// All edit states that should be given visible UI controls
        /// </summary>
        IList<EditState> VisibleEditStates { get; }
        /// <summary>
        /// All navigations that should be given visible UI controls
        /// </summary>
        IList<RecordNavigation> VisibleNavigationStates { get; }
    }
}