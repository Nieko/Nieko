using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Web.Modularity
{
    public class ModuleInfo
    {
        internal Type ModuleType { get; private set; }
        internal UniqueCollection<Type> Dependancies { get; private set; }

        internal ModuleInfo(Type moduleType)
        {
            ModuleType = moduleType;
            Dependancies = new UniqueCollection<Type>();
        }

        public ModuleInfo DependsOn<Dependancy>() where Dependancy : IModule
        {
            Dependancies.Add(typeof(Dependancy));

            return this;
        }
    }
}
}
