using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.Menus
{
    public interface IMenu
    {
        /// <summary>
        /// Display text
        /// </summary>
        string Caption { get;set;}
        /// <summary>
        /// Menu of which this is a child in the menu tree
        /// </summary>
        IMenu Parent { get; set; }
        /// <summary>
        /// Display / processing precedence among sibling menu items
        /// </summary>
        int Position { get; set; }
        /// <summary>
        /// Visible and functional status of this menu item
        /// </summary>
        MenuState State { get; }
        /// <summary>
        /// All menus that have this as their parent
        /// </summary>
        IList<IMenu> SubMenus { get;}
        /// <summary>
        /// Requests a navigation to the End Point represented by this
        /// menu item
        /// </summary>
        /// <returns>True if successful; false if another object cancels the navigation</returns>
        bool Navigate();
    }
}