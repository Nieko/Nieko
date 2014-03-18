using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Navigation.Menus
{
    /// <summary>
    /// Managing class for application menus
    /// </summary>
    public interface IMenuNavigator
    {
        /// <summary>
        /// Requests a navigation to the End Point 
        /// menu item
        /// </summary>
        /// <returns>True if successful; false if another object cancels the navigation</returns>
        bool NavigateTo(EndPoint destination);
        /// <summary>
        /// Raised when the <see cref="CurrentMenu"/> changes
        /// </summary>
        event EventHandler CurrentMenuChanged;
        /// <summary>
        /// Menu item currently navigated to
        /// </summary>
        IMenu CurrentMenu { get;}
        /// <summary>
        /// Top-most menu of the menu tree hierarchy
        /// </summary>
        IMenu RootMenu { get; }
    }
}