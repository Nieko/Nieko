using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.Menus;
using System.Security.Principal;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Provides security details for <see cref="IMenu"/> instances
    /// </summary>
    public interface IMenuAccessManager
    {
        /// <summary>
        /// Indicates whether a disabled menu item should be visibly
        /// disabled or hidden
        /// </summary>
        /// <param name="menu">Menu item</param>
        /// <returns>True if hidden when disabled</returns>
        bool HideIfDisabled(IMenu menu);
        /// <summary>
        /// Indicates whether a user has access to a menu
        /// </summary>
        /// <param name="menu">Menu item</param>
        /// <param name="principal">User security details</param>
        /// <returns>True if access granted</returns>
        bool HasAccess(IMenu menu, IPrincipal principal);
        /// <summary>
        /// Update security and menu details from respective declarations
        /// </summary>
        void UpdateFromDeclarations();
    }
}
