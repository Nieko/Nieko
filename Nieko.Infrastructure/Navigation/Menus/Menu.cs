using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.Menus
{
    public static class MenuExtensions
    {
        /// <summary>
        /// Converts a menu item into a forward-slash delimited string
        /// indicating the full path to navigate to it
        /// </summary>
        /// <param name="menu">Current instance</param>
        /// <returns>Navigation path</returns>
        public static string GetFullPath(this IMenu menu)
        {
            if (menu.Parent == null)
            {
                return "/";
            }

            string path = "";
            var currentMenu = menu;

            while (currentMenu != null)
            {
                if (currentMenu.Parent != null)
                {
                    path = "/" + currentMenu.Caption + path;
                }
                currentMenu = currentMenu.Parent;
            }

            return path;
        }
    }
}
