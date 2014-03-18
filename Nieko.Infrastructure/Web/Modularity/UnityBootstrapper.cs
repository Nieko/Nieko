using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Web.Services;

namespace Nieko.Infrastructure.Web.Modularity
{
    public abstract class UnityBootstrapper
    {
        protected IUnityContainer Container { get; private set; }

        public virtual IModuleLoader ModuleLoader { get; private set; }

        public void Run()
        {
            Container = CreateContainer();
            ModuleLoader = CreateModuleLoader();

            ModuleLoader.Load();
            AddDefaultRegistrations();

            Container.Resolve<IInitializedPluginFinder>().Initialize(); 
            RunStartupRoutines();
        }

        protected virtual void AddDefaultRegistrations()
        {
            Container.RegisterInstance<Func<Type, IPlugInFactory>>((type) =>
            {
                var factoryType = typeof(PlugInFactory<>).MakeGenericType(type);
                return (IPlugInFactory)Container.Resolve(factoryType);
            });
 
            var pluginFinder = Container.Resolve<IInitializedPluginFinder>();

            Container.RegisterSingleton(typeof(IPluginFinder), pluginFinder.GetType());
            ServiceProvider.PluginFinder = pluginFinder;
        }

        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected abstract IModuleLoader CreateModuleLoader();

        private void RunStartupRoutines()
        {
            var routines = AssemblyHelper.FindTypes(t => t.GetInterface(typeof(IRunAtStartup).FullName) != null
                && !t.IsInterface
                && !t.IsAbstract)
                .Select(t =>
                    {
                        Container.RegisterType(t, t);
                        return Container.Resolve(t) as IRunAtStartup;
                    });

            foreach (var routine in routines)
            {
                routine.Run();
            }
        }
    }
}
}
