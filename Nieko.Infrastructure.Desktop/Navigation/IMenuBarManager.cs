using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Nieko.Infrastructure.ComponentModel;
using System.Windows;
using System.ComponentModel;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Facade for creating Windows menu control items
    /// </summary>
    public interface IMenuBarManager : INotifyPropertyChanged
    {
        /// <summary>
        /// The main Windows menu UI Control
        /// </summary>
        Menu MenuBar { get; set; }
        /// <summary>
        /// </summary>
        bool IsVisible { get; set; }
        /// <summary>
        /// Create a new main menu item
        /// </summary>
        /// <param name="path">Forward slash delimited path to new menu item</param>
        /// <param name="initialization">Additional actions to perform after menu item
        /// is initialized</param>
        /// <param name="owner">Destination that the menu is associated with. 
        /// EndPoint.Root indicates a global menu</param>
        /// <param name="eventMethod">Action to perform from menu</param>
        /// <returns>ICommand to <paramref name="eventMethod"/></returns>
        ICommand Create(string path, Action<MenuItem> initialization, EndPoint owner, Action<object> eventMethod);
        /// <summary>
        /// Create a new main menu item
        /// </summary>
        /// <param name="path">Forward slash delimited path to new menu item</param>
        /// <param name="owner">Destination that the menu is associated with. 
        /// EndPoint.Root indicates a global menu</param>
        /// <param name="eventMethod">Action to perform from menu</param>
        /// <returns>ICommand to <paramref name="eventMethod"/></returns>
        ICommand Create(string path, EndPoint owner, Action<object> eventMethod);
        /// <summary>
        /// Create a new main menu item
        /// </summary>
        /// <param name="path">Forward slash delimited path to new menu item</param>
        /// <param name="owner">Destination that the menu is associated with. 
        /// EndPoint.Root indicates a global menu</param>
        /// <param name="command">Command executed by menu</param>
        /// <returns><paramref name="command"/></returns>
        ICommand Create(string path, EndPoint owner, ICommand command);
        /// <summary>
        /// Create a new main menu item
        /// </summary>
        /// <param name="path">Forward slash delimited path to new menu item</param>
        /// <param name="initialization">Additional actions to perform after menu item
        /// is initialized</param>
        /// <param name="owner">Destination that the menu is associated with. 
        /// EndPoint.Root indicates a global menu</param>
        /// <param name="command">Command executed by menu</param>
        /// <returns><paramref name="command"/></returns>
        ICommand Create(string path, Action<MenuItem> initialization, EndPoint owner, ICommand command);
    }
}
