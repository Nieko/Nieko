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
        private IEnumerable<IDataNavigatorOwner> _Children; 

        /// <summary>
        /// Parent of Children enumerated by this class. The Parent
        /// property Hierarchy references back to the same instance.
        /// </summary>
        public IDataNavigatorOwner Parent { get; internal set; }

        internal Action<IDataNavigatorOwner> ChildAddition { get; set;}
        internal Action<IDataNavigatorOwner> ChildRemoval { get; set; }

        internal OwnershipHierarchy(HashSet<IDataNavigatorOwner> children)
        {
            _Children = children;
            ChildAddition = o => { };
            ChildRemoval = o => { };
        }

        public void AddChild(IDataNavigatorOwner child)
        {
            ChildAddition(child); 
        }

        public void RemoveChild(IDataNavigatorOwner child)
        {
            ChildRemoval(child);
        }

        /// <summary>
        /// Allows enumeration through Children of the <see cref="Parent"/> IDataNavigatorOwner
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IDataNavigatorOwner> GetEnumerator()
        {
            return _Children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Children.GetEnumerator(); 
        }
    }
}
