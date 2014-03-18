using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Base class for defining a Dependency Injected Module
    /// </summary>
    /// <remarks>
    /// Implemented by a single class within an assembly to detail how that 
    /// assembly will be loaded as a Module. Modules do not have references or 
    /// knowledge of other Modules, reducing coupling and cohesion. Modules
    /// make appropriate functionality available by implementing interfaces
    /// within Infrastructure assemblies (i.e. EndPoints, Aggregated Events)
    /// </remarks>
    public abstract class ContainerModule : IContainerModule
    {
        private DependantModule _Dependancies;

        public IContainerAdapter Container { get; set; }
        
        public DependantModule Dependant 
        {
            get
            {
                if (_Dependancies == null)
                {
                    _Dependancies = GetDependancies();
                }

                return _Dependancies;
            }
        }

        /// <summary>
        /// Used to build dependency details; should be called from the implementation
        /// of <see cref="GetDependancies"/>.
        /// </summary>
        /// <typeparam name="TNames">Class in which this Module's name is defined</typeparam>
        /// <param name="moduleName">Expression to current Module's name</param>
        /// <returns>Dependency details (with fluent interface for further elaboration)</returns>
        protected DependantModule CreateDependancies<TNames>(Expression<Func<TNames, string>> moduleName)
        {
            return new DependantModule(typeof(TNames).FullName + "." + BindingHelper.Name(moduleName));   
        }

        public abstract void Initialize();
        protected abstract DependantModule GetDependancies();
    }
}
