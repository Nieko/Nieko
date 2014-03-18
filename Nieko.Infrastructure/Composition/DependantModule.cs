using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Nieko.Infrastructure.Composition
{
    /// <summary>
    /// Encapsulates detail of dependencies for a Module
    /// </summary>
    public sealed class DependantModule
    {
        private List<KeyValuePair<Type, string>> _Dependancies = new List<KeyValuePair<Type,string>>(); 

        public ReadOnlyCollection<KeyValuePair<Type, string>> Dependancies { get; private set; }

        public string Name { get; private set; }

        internal DependantModule(string name)
        {
            Name = name;

            Dependancies = new ReadOnlyCollection<KeyValuePair<Type, string>>(_Dependancies);
        }

        /// <summary>
        /// Fluent method for adding dependency on another Module.
        /// </summary>
        /// <remarks>
        /// <typeparamref name="TName"/> does not have to be the same class as that providing
        /// the name for the current ContainerModule. This allows for non-coupled tiers of Modules to be
        /// created. Module names can be from different classes; but most importantly by only referencing
        /// the Module name 
        /// </remarks>
        /// <typeparam name="TName">Class containing name of depended Module</typeparam>
        /// <param name="dependancy">Expression to depended Module name</param>
        /// <returns>Current instance</returns>
        public DependantModule AddDependancy<TName>(Expression<Func<TName, string>> dependancy)
        {
            _Dependancies.Add(new KeyValuePair<Type, string>(typeof(TName), BindingHelper.Name(dependancy)));  

            return this;
        }


    }
}
