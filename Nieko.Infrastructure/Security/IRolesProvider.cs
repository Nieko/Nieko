using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Source for all Security Roles
    /// </summary>
    public interface IRolesProvider
    {
        /// <summary>
        /// All roles within system
        /// </summary>
        IList<IRole> Roles { get; }
        /// <summary>
        /// Determines all roles that a role participates in
        /// </summary>
        /// <param name="role">Role to retrieve membership for</param>
        /// <returns>Effective memberships keyed by name</returns>
        IDictionary<string, IRole> GetEffectiveMemberships(IRole role);
    }
}