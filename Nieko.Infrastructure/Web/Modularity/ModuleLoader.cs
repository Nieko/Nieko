using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Web.Modularity
{
    public class ModuleLoader : IModuleLoader
    {
        private readonly IUnityContainer _Container;
        private UniqueCollection<ModuleInfo> _RegisteredModules;
        private UniqueCollection<Type> _LoadedModules;

        internal ModuleLoader(IUnityContainer container)
        {
            _Container = container;
            _RegisteredModules = new UniqueCollection<ModuleInfo>();
            _LoadedModules = new UniqueCollection<Type>();
        }

        public ModuleInfo Add<TModule>()
            where TModule : IModule
        {
            return new ModuleInfo(typeof(TModule));
        }

        public void Load()
        {
            var unprocessedModules = _RegisteredModules.ToList();
            int lastCount = unprocessedModules.Count;
            IModule module;

            while (unprocessedModules.Count != 0)
            {
                foreach (var moduleInfo in unprocessedModules
                    .Where(unprocessed => unprocessed.Dependancies
                        .Any(dependancy => !_LoadedModules.Contains(dependancy)))
                    .ToList())
                {
                    _Container.RegisterType(moduleInfo.ModuleType);
                    module = _Container.Resolve(moduleInfo.ModuleType) as IModule;
                    module.Initialize();

                    _LoadedModules.Add(moduleInfo.ModuleType);
                    unprocessedModules.Remove(moduleInfo);
                }

                if (unprocessedModules.Count == lastCount)
                {
                    throw new ModuleLoadException("Unable to resolve dependancies; possible circular dependacy definitions"); 
                }

                lastCount = unprocessedModules.Count;
            }
        }
    }
}
}
