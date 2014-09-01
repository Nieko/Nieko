using System;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.Collections.Generic;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public interface IOwnershipHierarchy : IEnumerable<ITierCoordinator>
    {
        /// <summary>
        /// Parent of Children enumerated by this class. The Parent
        /// property Hierarchy references back to the same instance.
        /// </summary>
        ITierCoordinator Parent { get; }
        /// <summary>
        /// Sets a parent-child relationship between the Owner of this instance
        /// and <paramref name="child"/>
        /// </summary>
        /// <param name="child">Instance to add as child</param>
        void AddChild(ITierCoordinator child);
        /// <summary>
        /// Removes a parent-child relationship between the Owner of this instance
        /// and <paramref name="child"/>
        /// </summary>
        /// <param name="child">Instance to remove as child</param>
        void RemoveChild(ITierCoordinator child);
    }
}
