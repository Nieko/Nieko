using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Security;

namespace Nieko.Modules.Security
{
    public class AttributeRolesProvider : RolesProviderBase
    {
        private bool _IsInitialised = false;
        private List<IRole> _Roles = new List<IRole>();

        public AttributeRolesProvider()
        {
        }

        public override IList<IRole> Roles
        {
            get
            {
                return _Roles;
            }
        }

        public void Initialize()
        {
            if (_IsInitialised)
            {
                return;
            }
            var containers = AssemblyHelper.FindTypes(t => Attribute.IsDefined(t, typeof(RoleContainerAttribute), false));
            var roleProperties = containers.SelectMany(t => t.GetProperties(BindingFlags.Static | BindingFlags.Public)
                                                                .Where(p => p.PropertyType.GetInterface(typeof(IRole).FullName) != null));

            foreach (var property in roleProperties)
            {
                Roles.Add((IRole)property.GetValue(null, null));
            }

            PopulateEffectiveMembershipsByRole();

            _IsInitialised = true;
        }

    }
}