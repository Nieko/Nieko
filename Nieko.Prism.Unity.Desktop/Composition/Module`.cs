using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Composition;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace Nieko.Prism.Composition
{
    internal class Module<T> : IModule where T : IContainerModule, new()
    {
        private IContainerAdapter _Container;

        public Module(IUnityContainer container)
        {
            _Container = (IContainerAdapter)container;
        }

        public void Initialize()
        {
            var containerModule = new T();

            containerModule.Container = _Container;
            containerModule.Initialize(); 
        }
    }
}
