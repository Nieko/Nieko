using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    /// <summary>
    /// Details parent-child relationships between
    /// <seealso cref="IDataNavigationOwner"/> instances
    /// </summary>
    /// <remarks>
    /// Persistence and relationship operations are not exposed for use outside
    /// of the framework as their use is only appropriate internally
    /// </remarks>
    public sealed class OwnershipHierarchy : IOwnershipHierarchy
    {
        private IEnumerable<ITierCoordinator> _Children; 

        /// <summary>
        /// Parent of Children enumerated by this class. The Parent
        /// property Hierarchy references back to the same instance.
        /// </summary>
        public ITierCoordinator Parent { get; internal set; }

        internal Action<ITierCoordinator> ChildAddition { get; set;}
        internal Action<ITierCoordinator> ChildRemoval { get; set; }

        internal OwnershipHierarchy(HashSet<ITierCoordinator> children)
        {
            _Children = children;
            ChildAddition = o => { };
            ChildRemoval = o => { };
        }

        public void AddChild(ITierCoordinator child)
        {
            ChildAddition(child); 
        }

        public void RemoveChild(ITierCoordinator child)
        {
            ChildRemoval(child);
        }

        /// <summary>
        /// Allows enumeration through Children of the <see cref="Parent"/> ITierCoordinator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ITierCoordinator> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Children.GetEnumerator(); 
        }
    }
}
