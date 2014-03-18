using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Dependency Injection Container
    /// </summary>
    /// <remarks>
    /// Reduces coupling on IoC Container implementation. Modules not implementing core
    /// framework functionality can utilize Modular Dependency Injection without needing 
    /// to reference the assemblies that actually implement Dependency Injection i.e.
    /// Modules do not need to reference Prism / Ninject / Spring.NET etc. dlls.
    /// </remarks>
    public interface IContainerAdapter
    {
        bool IsRegistered<T>();
        bool IsRegistered<T>(string nameToCheck);
        bool IsRegistered(Type typeToCheck);
        bool IsRegistered(Type typeToCheck, string nameToCheck);
        IContainerAdapter RegisterInstance<TInterface>(TInterface instance);
        IContainerAdapter RegisterInstance<TInterface>(string name, TInterface instance);
        IContainerAdapter RegisterInstance(Type t, object instance);
        IContainerAdapter RegisterInstance(Type t, string name, object instance);
        IContainerAdapter RegisterSingleton<TFrom, TTo>() where TTo : TFrom;
        IContainerAdapter RegisterSingleton<TFrom, TTo>(bool initialize) where TTo : TFrom;
        IContainerAdapter RegisterSingleton<TFrom, TTo>(string name) where TTo : TFrom;
        IContainerAdapter RegisterSingleton<TFrom, TTo>(string name, bool initialize) where TTo : TFrom;
        IContainerAdapter RegisterSingleton(Type fromType, Type toType);
        IContainerAdapter RegisterSingleton(Type fromType, Type toType, bool initialize);
        IContainerAdapter RegisterSingleton(Type fromType, Type toType, string name);
        IContainerAdapter RegisterSingleton(Type fromType, Type toType, string name, bool initialize);
        IContainerAdapter RegisterFactory<TInterface>();
        IContainerAdapter RegisterType<TFrom, TTo>() where TTo : TFrom;
        IContainerAdapter RegisterType<TFrom, TTo>(string name) where TTo : TFrom;
        IContainerAdapter RegisterType(Type from, Type to);
        IContainerAdapter RegisterTypeAndFactory<TFrom, TTo>() where TTo : TFrom;
        IContainerAdapter RegisterTypeAndFactory<T>();
        T Resolve<T>();
        T Resolve<T>(string name);
        object Resolve(Type t);
    }
}
