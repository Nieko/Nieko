using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// IPersistedView[T] built using the IGraphFactory fluent interfaces
    /// </summary>
    /// <remarks>
    /// Interface primarily exists to provide methods for adding child leaves / tiers to
    /// the ModelView graph
    /// </remarks>
    /// <typeparam name="T">ModelView</typeparam>
    /// <typeparam name="TEntity">Entity to map to</typeparam>
    /// <typeparam name="TDataStore">Store holding Entity instances</typeparam>
    public interface IConstructedPersistedView<T, TEntity, TDataStore> : IConstructedPersistedView, IPersistedView<T>
        where T : IEditableMirrorObject
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        /// <summary>
        /// Adds a child tier representing a membership
        /// </summary>
        /// <typeparam name="TChildEntity">Member Entity</typeparam>
        /// <returns>Factory to build Membership Provider</returns>
        IMembershipProviderFactory<TEntity, TChildEntity, TDataStore> AddMembershipChild<TChildEntity>()
            where TChildEntity : class, new();
        /// <summary>
        /// Adds a child tier representing a relationship
        /// </summary>
        /// <typeparam name="TChildMirror">ModelView</typeparam>
        /// <typeparam name="TChildEntity">Entity to map to</typeparam>
        /// <returns>Completed Graph Factory</returns>
        IFinishedGraphFactory<TChildMirror, TChildEntity, TEntity, TDataStore> AddChild<TChildMirror, TChildEntity>()
            where TChildMirror : class, IEditableMirrorObject, new()
            where TChildEntity : class, new();
    }
}
