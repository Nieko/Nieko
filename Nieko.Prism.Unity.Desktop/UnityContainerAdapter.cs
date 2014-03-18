using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Nieko.Infrastructure.Composition;

namespace Nieko.Prism.Unity
{
    public class UnityContainerAdapter : UnityContainer, IContainerAdapter
    {
        public bool IsRegistered<T>()
        {
            return UnityContainerExtensions.IsRegistered<T>(this); 
        }

        public bool IsRegistered<T>(string nameToCheck)
        {
            return UnityContainerExtensions.IsRegistered<T>(this, nameToCheck);
        }

        public bool IsRegistered(Type typeToCheck)
        {
            return UnityContainerExtensions.IsRegistered(this, typeToCheck);
        }

        public bool IsRegistered(Type typeToCheck, string nameToCheck)
        {
            return UnityContainerExtensions.IsRegistered(this, typeToCheck, nameToCheck);
        }

        public IContainerAdapter RegisterInstance<TInterface>(TInterface instance)
        {
            UnityContainerExtensions.RegisterInstance<TInterface>(this, instance);

            return this;
        }

        public IContainerAdapter RegisterInstance<TInterface>(string name, TInterface instance)
        {
            UnityContainerExtensions.RegisterInstance<TInterface>(this, name, instance);

            return this;
        }

        public IContainerAdapter RegisterInstance(Type t, object instance)
        {
            UnityContainerExtensions.RegisterInstance(this, t, instance);

            return this;
        }

        public IContainerAdapter RegisterInstance(Type t, string name, object instance)
        {
            UnityContainerExtensions.RegisterInstance(this, t, name, instance);

            return this;
        }

        public IContainerAdapter RegisterSingleton<TFrom, TTo>()
            where TTo : TFrom
        {
            return RegisterSingleton<TFrom, TTo>(false);
        }

        public IContainerAdapter RegisterSingleton<TFrom, TTo>(bool initialize)
            where TTo : TFrom
        {
            return RegisterSingleton(typeof(TFrom), typeof(TTo), initialize);
        }

        public IContainerAdapter RegisterSingleton<TFrom, TTo>(string name)
            where TTo : TFrom
        {
            return RegisterSingleton<TFrom, TTo>(name, false);
        }

        public IContainerAdapter RegisterSingleton<TFrom, TTo>(string name, bool initialize)
            where TTo : TFrom
        {
            return RegisterSingleton(typeof(TFrom), typeof(TTo), name, initialize);
        }

        public IContainerAdapter RegisterSingleton(Type fromType, Type toType)
        {
            return RegisterSingleton(fromType, toType, false); 
        }

        public IContainerAdapter RegisterSingleton(Type fromType, Type toType, bool initialize)
        {
            UnityContainerExtensions.RegisterType(this, fromType, toType, new ContainerControlledLifetimeManager());

            if (initialize)
            {
                this.Resolve(fromType);
            }

            return this;
        }

        public IContainerAdapter RegisterSingleton(Type fromType, Type toType, string name)
        {
            return RegisterSingleton(fromType, toType, name, false);  
        }

        public IContainerAdapter RegisterSingleton(Type fromType, Type toType, string name, bool initialize)
        {
            base.RegisterType(fromType, toType, name, new ContainerControlledLifetimeManager(), new InjectionMember[]{});

            if (initialize)
            {
                Resolve(fromType, name);
            }

            return this;
        }

        public IContainerAdapter RegisterFactory<TInterface>()
        {
            return RegisterInstance<Func<TInterface>>(() => Resolve<TInterface>());
        }

        public IContainerAdapter RegisterTypeAndFactory<TFrom, TTo>()
            where TTo : TFrom
        {
            return RegisterType<TFrom, TTo>()
                .RegisterFactory<TFrom>(); 
        }

        public IContainerAdapter RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            UnityContainerExtensions.RegisterType<TFrom, TTo>(this);

            return this;
        }

        public T Resolve<T>()
        {
            return UnityContainerExtensions.Resolve<T>(this); 
        }

        public T Resolve<T>(string name)
        {
            return UnityContainerExtensions.Resolve<T>(this, name); 
        }

        public object Resolve(Type t)
        {
            return UnityContainerExtensions.Resolve(this, t); 
        }


        public IContainerAdapter RegisterType<TFrom, TTo>(string name) where TTo : TFrom
        {
            UnityContainerExtensions.RegisterType<TFrom, TTo>(this, name);

            return this;
        }

        public IContainerAdapter RegisterType(Type from, Type to)
        {
            UnityContainerExtensions.RegisterType(this, from, to);

            return this;
        }

        public IContainerAdapter RegisterTypeAndFactory<T>()
        {
            return RegisterTypeAndFactory<T, T>(); 
        }
    }
}
