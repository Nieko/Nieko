using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Nieko.Infrastructure.Reflection;
using System.Reflection;
using Nieko.Infrastructure.Composition;
using Nieko.Infrastructure;
using Nieko.Prism.Composition;

namespace Microsoft.Practices.Prism.Modularity
{
    public static class ModuleCatalogExtensions
    {
        public static IModuleCatalog AddModule<T>(this IModuleCatalog catalog) where T : IContainerModule, new()
        {
            var containerModule = new T();

            var dependancies = containerModule.Dependant.Dependancies;
            var name = containerModule.Dependant.Name;
            var moduleInfo = new ModuleInfo(name, typeof(Module<T>).AssemblyQualifiedName)
            {
                InitializationMode = InitializationMode.WhenAvailable,
                Ref = null
            };

            if (dependancies.Count != 0)
            {
                moduleInfo.DependsOn.AddRange(dependancies
                    .Select(d => d.Key.FullName + "." + d.Value)
                    .ToArray());
            }

            catalog.AddModule(moduleInfo);

            return catalog;
        }

        public static IModuleCatalog AddContainerModule(this IModuleCatalog catalog, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type"); 
            }

            if (!typeof(IContainerModule).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition
                || type.GetConstructor(Type.EmptyTypes) == null) 
            {
                throw new ArgumentException(type.FullName + " does not have a parameterless constructor"); 
            }

            var implementation = typeof(ModuleCatalogExtensions).GetMethod("AddModule", BindingFlags.Static | BindingFlags.Public);
            implementation = implementation.MakeGenericMethod(type);
            var result = implementation.Invoke(null, new object[]{ catalog });

            return (IModuleCatalog)result;
        }
    }
}
