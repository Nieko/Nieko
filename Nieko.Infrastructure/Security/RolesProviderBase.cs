using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Security;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Base class for implementing IRolesProvider.
    /// </summary>
    /// <remarks>
    /// Implements role membership discovery; inheriting classes need only
    /// implement population of role instances from whatever source is appropriate 
    /// to it
    /// </remarks>
    public abstract class RolesProviderBase : IRolesProvider
    {
        private Dictionary<IRole, Dictionary<string, IRole>> _EffectiveMembershipsByRole = new Dictionary<IRole, Dictionary<string, IRole>>();

        protected Dictionary<IRole, Dictionary<string, IRole>> EffectiveMembershipsByRole
        {
            get { return _EffectiveMembershipsByRole; }
            set { _EffectiveMembershipsByRole = value; }
        }

        public abstract IList<IRole> Roles { get; }

        public virtual IDictionary<string, IRole> GetEffectiveMemberships(IRole role)
        {
            return EffectiveMembershipsByRole[role];
        }

        protected void PopulateEffectiveMembershipsByRole()
        {
            Dictionary<string, IRole> effectiveMemberships;

            foreach (var role in Roles)
            {
                effectiveMemberships = new Dictionary<string, IRole>();
                AddMemberships(role, effectiveMemberships);
                RemoveExclusions(role, effectiveMemberships, new HashSet<IRole>(), new HashSet<IRole>());
                _EffectiveMembershipsByRole.Add(role, effectiveMemberships);
            }
        }

        private void AddMemberships(IRole role, Dictionary<string, IRole> roles)
        {
            roles.Add(role.Name, role);

            foreach (var membership in role.RoleMemberships.Values)
            {
                if (roles.ContainsKey(membership.Name))
                {
                    continue;
                }

                AddMemberships(membership, roles);
            }
        }

        private void RemoveExclusions(IRole role, Dictionary<string, IRole> roles, HashSet<IRole> parsedMemberships, HashSet<IRole> parsedExclusions)
        {
            foreach (var exclusion in role.RoleExclusions.Values)
            {
                if (parsedExclusions.Contains(exclusion))
                {
                    continue;
                }
                if (roles.ContainsKey(exclusion.Name))
                {
                    roles.Remove(exclusion.Name);
                    parsedExclusions.Add(exclusion);
                }
                RemoveExclusions(exclusion, roles, parsedMemberships, parsedExclusions);
            }

            foreach (var membership in role.RoleMemberships.Values)
            {
                if (parsedMemberships.Contains(membership))
                {
                    continue;
                }
                parsedMemberships.Add(membership);
                RemoveExclusions(membership, roles, parsedMemberships, parsedExclusions);
            }
        }

    }
}