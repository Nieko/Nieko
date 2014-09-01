using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel.ViewModelling;
using Nieko.Infrastructure.ComponentModel;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Graph factory that is ready to construct all objects
    /// for this tier / leaf of the ModelView graph
    /// </summary>
    /// <typeparam name="T">ModelView to map from</typeparam>
    /// <typeparam name="TEntity">Entity to map to</typeparam>
    /// <typeparam name="TDataStore">Store containing TEntity instances</typeparam>
    public interface IFinishedGraphFactory<T, TEntity, TDataStore>
        where T : class, IEditableMirrorObject, new()
        where TEntity : class, new()
        where TDataStore : class, IDataStore
    {
        /// <summary>
        /// Allows additional ModelView creation action(s) to be provided -after- the <seealso cref="IPersistedView"/> 
        /// has initialized. 
        /// </summary>
        /// <remarks>
        /// i.e. only perform expensive actions if _DisplayGlyphs flag set during load
        /// () =>   {
        ///             if(_DisplayGlyphs)
        ///             {
        ///                 return (li, o) => { LoadLineItemImages(li, o.GlyphStyle.Name); };
        ///             }
        ///             else
        ///             {
        ///                 return (li, o) => { };
        ///             }
        ///         }
        /// </remarks>
        /// <param name="mirroringBuilder">Delegate to build ModelView creation action(s)</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> BuildMirroringBy(Func<Action<T, TEntity>> mirroringBuilder);
        /// <summary>
        /// Specifies alternate setup of ModelView-Entity TypeMapper to that of
        /// implying all congruent properties on both types
        /// </summary>
        /// <param name="initializer">Initialization</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> InitializeTypeMapperBy(Action<ITypeMapper<T, TEntity>> initializer);
        /// <summary>
        /// Provides additional actions to perform when a new ModelView
        /// is added (i.e. a line item representing a new entity instance)
        /// </summary>
        /// <param name="initializer">Initialization action</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> InitializingNewItemBy(Action<T> initializer);
        /// <summary>
        /// Specifies additional setup actions for ListCollectionView of this tier
        /// </summary>
        /// <param name="initializer">Additional actions</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> InitializingViewBy(Action<ICollectionViewWrapper> initializer);
        /// <summary>
        /// Provides alternative Entity Expression builder to retrieve Entities with
        /// </summary>
        /// <param name="entityQuery">Expression builder</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> WithEntityQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> entityQuery);
        /// <summary>
        /// Provides post-mirroring filtering of ModelView line-items
        /// </summary>
        /// <param name="itemsQuery">Line item filter</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> WithItemsQuery(Func<IEnumerable<T>, IEnumerable<T>> itemsQuery);
        /// <summary>
        /// Provides Navigation UI Control configuration
        /// </summary>
        /// <param name="viewConfig">Configuration</param>
        /// <returns>Current Instance</returns>
        IFinishedGraphFactory<T, TEntity, TDataStore> WithUIConfig(IUIConfig viewConfig);
        /// <summary>
        /// Build IPersistedView using parameters provided to this factory
        /// </summary>
        /// <returns>IPersistedView for this tier</returns>
        IConstructedPersistedView<T, TEntity, TDataStore> Build();
    }
}