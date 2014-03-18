using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Role-based security manager
    /// </summary>
    /// <remarks>
    /// Utilizes basic functionality provided via System.Security interfaces.
    /// </remarks>
    public interface ISecurityManager
    {
        IPrincipal CurrentUser { get; }
        IDictionary<string, IRole> RolesByName { get; }
        IRolesProvider RolesProvider { get; }
        /// <summary>
        /// Determines if a user participates in a role
        /// </summary>
        /// <param name="role">Security role</param>
        /// <param name="principal">User</param>
        /// <returns>True if participates</returns>
        bool IsInRole(IRole role, IPrincipal principal);
    }
}