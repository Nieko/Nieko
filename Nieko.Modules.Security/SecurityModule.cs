using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Security;
using Nieko.Infrastructure.Composition;
using System.Linq.Expressions;

namespace Nieko.Modules.Security
{
    public class SecurityModule : ContainerModule
    {
        public override void Initialize() 
        {
            var rolesProvider = new AttributeRolesProvider();

            rolesProvider.Initialize();

            Container.RegisterInstance<IRolesProvider>(rolesProvider);
        }

        protected override DependantModule GetDependancies()
        {
            return CreateDependancies<NiekoModuleNames>(mn => mn.Security);
        }
    }
}
