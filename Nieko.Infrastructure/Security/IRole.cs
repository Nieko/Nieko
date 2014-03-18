using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Security role participated in by users
    /// or demanded by components
    /// </summary>
    public interface IRole
    {
        string Name { get; set; }
        /// <summary>
        /// Other roles that this role cannot participate in
        /// </summary>
        IDictionary<string, IRole> RoleExclusions { get; }
        /// <summary>
        /// Roles that this role includes permissions from.
        /// </summary>
        /// <remarks>
        /// Roles of which this is a member recursively aggregates memberships and then
        /// recursively removes -all- exclusions, i.e. exclusions take precedence over
        /// memberships
        /// </remarks>
        IDictionary<string, IRole> RoleMemberships { get; }
    }
}