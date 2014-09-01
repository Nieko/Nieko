using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// MVVM bindable implementation of IDataNavigator. Used by 
    /// implementers of <seealso cref="IDataNavigatorView"/> as their
    /// backing ViewModel
    /// </summary>
    public interface IDataNavigatorViewModel : IDataNavigator
    {
        /// <summary>
        /// Cancels current editing operations
        /// </summary>
        ICommand Cancel { get; }
        /// <summary>
        /// Deletes current instance
        /// </summary>
        ICommand Delete { get; }
        /// <summary>
        /// Manually enters edit state
        /// </summary>
        ICommand Edit { get; }
        /// <summary>
        /// Navigate to first item
        /// </summary>
        ICommand First { get; }
        /// <summary>
        /// Navigate to specific index
        /// </summary>
        ICommand GoToPosition { get; }
        /// <summary>
        /// Navigate to last item
        /// </summary>
        ICommand Last { get; }
        /// <summary>
        /// Creates a new item and begins editing
        /// </summary>
        ICommand New { get; }
        /// <summary>
        /// Navigate to next item
        /// </summary>
        ICommand Next { get; }
        /// <summary>
        /// Navigate back to previous item
        /// </summary>
        ICommand Previous { get; }
        /// <summary>
        /// Save current changes or new item
        /// </summary>
        ICommand Save { get; }
        /// <summary>
        /// Open UI for editing current filters and sorting
        /// </summary>
        ICommand ChangeSearchFilter { get; }
        /// <summary>
        /// Visibility of Cancel UI element(s)
        /// </summary>
        Visibility CancelVisible { get; set;}
        /// <summary>
        /// Visibility of item count UI element(s)
        /// </summary>
        Visibility CountVisible { get; set; }
        /// <summary>
        /// Visibility of delete UI element(s)
        /// </summary>
        Visibility DeleteVisible { get; set; }
        /// <summary>
        /// Visibility of edit UI element(s)
        /// </summary>
        Visibility EditVisible { get; set; }
        /// <summary>
        /// Visibility of navigate to first item UI element(s)
        /// </summary>
        Visibility FirstVisible { get; set; }
        /// <summary>
        /// Visibility of navigate to position first UI element(s)
        /// </summary>
        Visibility GoToPositionVisible { get; set; }
        /// <summary>
        /// Visibility of navigate to last item UI element(s)
        /// </summary>
        Visibility LastVisible { get; set; }
        /// <summary>
        /// Visibility of edit new item UI element(s)
        /// </summary>
        Visibility NewVisible { get; set; }
        /// <summary>
        /// Visibility of navigate to next item UI element(s)
        /// </summary>
        Visibility NextVisible { get; set; }
        /// <summary>
        /// Visibility of navigate to previous item UI element(s)
        /// </summary>
        Visibility PreviousVisible { get; set; }
        /// <summary>
        /// Visibility of save item UI element(s)
        /// </summary>
        Visibility SaveVisible { get; set; }
        /// <summary>
        /// Visibility of entire toolbar containing Navigation controls 
        /// </summary>
        Visibility ToolBarVisibility { get; set; }
        /// <summary>
        /// Commands to enter edit state keyed by <seealso cref="EditState"/>  
        /// </summary>
        Dictionary<EditState, ICommand> EditStateCommands { get; }
        /// <summary>
        /// Edit actions that can currently be performed
        /// </summary>
        ObservableCollection<EditState> EditStatesEnabled { get; }
        /// <summary>
        /// Edit actions that have visible UI Elements
        /// </summary>
        ObservableCollection<EditState> EditStatesVisible { get; }
        /// <summary>
        /// Navigation commands keyed by type of navigation
        /// </summary>
        Dictionary<RecordNavigation, ICommand> NavigationCommands { get; }
        /// <summary>
        /// Navigations that are currently enabled
        /// </summary>
        ObservableCollection<RecordNavigation> NavigationsEnabled { get; }
        /// <summary>
        /// Navigations that have visible UI Elements
        /// </summary>
        ObservableCollection<RecordNavigation> NavigationsVisible { get; }
        /// <summary>
        /// Indicates whether or not to hide all Navigation UI elements
        /// </summary>
        bool ShowNavigators { get; set; }
        /// <summary>
        /// Provides visibility settings for all navigation and
        /// editing controls
        /// </summary>
        INavigatorVisibilityProvider VisibilityProvider { get; set; }
    }
}