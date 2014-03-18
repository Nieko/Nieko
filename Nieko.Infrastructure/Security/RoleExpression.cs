using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure.Security;
using Nieko.Infrastructure;

namespace Nieko.Infrastructure.Security
{
    /// <summary>
    /// Role builder using self-referencing expressions
    /// </summary>
    public class RoleExpression : IRole
    {
        private string _Name;
        private IDictionary<string, IRole> _RoleExclusions;
        private IDictionary<string, IRole> _RoleMemberships;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleAccessor">Role Property Accessor this instance will be returned by</param>
        /// <param name="memberships">Other roles to participate in</param>
        /// <param name="exclusions">Roles of which this role is excluded</param>
        public RoleExpression(Expression<Func<IRole>> roleAccessor, IRole[] memberships, IRole[] exclusions)
        {
            Name = BindingHelper.Name(roleAccessor);

            RoleMemberships = memberships.ToDictionary(r => r.Name);
            RoleExclusions = exclusions.ToDictionary(r => r.Name);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleAccessor">Role Property Accessor this instance will be returned by</param>
        /// <param name="memberships">Other roles to participate in</param>
        public RoleExpression(Expression<Func<IRole>> roleAccessor, IRole[] memberships)
            : this(roleAccessor, memberships, new IRole[]{})
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleAccessor">Role Property Accessor this instance will be returned by</param>
        public RoleExpression(Expression<Func<IRole>> roleAccessor)
            : this(roleAccessor, new IRole[] { })
        {
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public IDictionary<string, IRole> RoleExclusions
        {
            get
            {
                return _RoleExclusions;
            }
            private set
            {
                _RoleExclusions = value;
            }
        }

        public IDictionary<string, IRole> RoleMemberships
        {
            get
            {
                return _RoleMemberships;
            }
            private set
            {
                _RoleMemberships = value;
            }
        }

    }
}