using System;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Data;
using System.Linq.Expressions;
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
    /// <typeparam name="TParentEntity">Entity in the tier / leaf above with n-ary relationship to TEntity</typeparam>
    /// <typeparam name="TDataStore">Store containing TEntity instances</typeparam>
    public interface IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore>
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
        IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> BuildMirroringBy(Func<Action<T, TEntity>> mirroringBuilder);
        /// <summary>
        /// Specifies alternate setup of ModelView-Entity TypeMapper to that of
        /// implying all congruent properties on both types
        /// </summary>
        /// <param name="initializer">Initialization</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> InitializeTypeMapperBy(Action<ITypeMapper<T, TEntity>> initializer);
        /// <summary>
        /// Provides additional actions to perform when a new ModelView
        /// is added (i.e. a line item representing a new entity instance)
        /// </summary>
        /// <param name="initializer">Initialization action</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> InitializingNewItemBy(Action<T> initializer);
        /// <summary>
        /// Specifies additional setup actions for ListCollectionView of this tier
        /// </summary>
        /// <param name="initializer">Additional actions</param>
        /// <returns>Current instance</returns>
        IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> InitializingViewBy(Action<ICollectionViewWrapper> initializer);
        /// <summary>
        /// Provides Navigation UI Control configuration. The default UI configuration
        /// is to not show a Navigation Control
        /// </summary>
        /// <param name="viewConfig">Configuration</param>
        /// <returns>Current Instance</returns>
        IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> WithUIConfig(IUIConfig viewConfig);
        /// <summary>
        /// Builds the IPersistedView using factory and method parameters specified
        /// </summary>
        /// <remarks>
        /// <paramref name="parentSetter"/> and <paramref name="parentFilter"/> are passed to this method rather 
        /// than having their own respective fluent methods because these parameters 
        /// are required for IPersistedView construction whereas all other fluent
        /// methods are optional
        /// </remarks>
        /// <param name="parentSetter">Method to set reference on <typeparamref name="TEntity"/> to parent <typeparamref name="TParentEntity"/></param>
        /// <param name="parentFilter">Method to find all <typeparamref name="TEntity"/> instances with parent <typeparamref name="TParentEntity"/></param>
        /// <returns>IPersistedView for this tier</returns>
        IConstructedPersistedView<T, TEntity, TDataStore> Build(Action<TEntity, TParentEntity> parentSetter, Func<TParentEntity, Expression<Func<TEntity, bool>>> parentFilter);
        /// <summary>
        /// Builds the IPersistedView using factory and method parameters specified
        /// </summary>
        /// <remarks>
        /// <paramref name="parentSetter"/> and <paramref name="parentFilter"/> are passed to this method rather 
        /// than having their own respective fluent methods because these parameters 
        /// are required for IPersistedView construction whereas all other fluent
        /// methods are optional
        /// </remarks>
        /// <param name="parentSetter">Method to set reference on <typeparamref name="TEntity"/> to parent <typeparamref name="TParentEntity"/></param>
        /// <param name="parentFilter">Method to find all <typeparamref name="TEntity"/> instances with parent <typeparamref name="TParentEntity"/></param>
        /// <param name="cascadeDelete">Delete related items when parent(s) are deleted</param>
        /// <returns>IPersistedView for this tier</returns>
        IConstructedPersistedView<T, TEntity, TDataStore> Build(Action<TEntity, TParentEntity> parentSetter, Func<TParentEntity, Expression<Func<TEntity, bool>>> parentFilter, bool cascadeDelete);
    }
}
