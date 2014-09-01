using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.Linq.Expressions;
using System.Windows.Data;
using System.ComponentModel;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure;

namespace Nieko.Modules.Navigation.Data
{
    /// <summary>
    /// Final fluent method set for a IGraphFactory[Owner] building a
    /// many-to-many tier
    /// </summary>
    /// <remarks>
    /// The root in a ModelView graph is a singleton placeholder, not mirroring
    /// entity data and so the first tier constructed will always be via 
    /// one-to-many FinishedGraphFactory[T, Owner, TEntity, TDataStore]
    /// </remarks>
    /// <typeparam name="T">ModelView</typeparam>
    /// <typeparam name="TEntity">Mirrored entity</typeparam>
    /// <typeparam name="TDataStore">Store</typeparam>
    internal class FinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> : IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore>
        where T : class, IEditableMirrorObject, new()
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        /// <summary>
        /// Internal implementation of IPersistedView, built by containing FinishedGraphFactory class
        /// Provides fluent method for creating child tiers
        /// </summary>
        /// <remarks>
        /// Use of nested class allows for simplification of generic type arguments and internal IPersistedView
        /// construction without sacrificing strong-typing and simplicity in Fluent Factory methods
        /// </remarks>
        public class ConstructedPersistedView : PersistedView<T, TEntity, TParentEntity, TDataStore>, IConstructedPersistedView<T, TEntity, TDataStore>
        {
            private ITypeMapper<T, TEntity> _TypeMapper;
            private Action<T, TEntity> _Mirroring = null;
            private Action<TEntity, TParentEntity> _SetParent = null;
            private Func<TParentEntity, Expression<Func<TEntity, bool>>> _ParentFilter = null;
            private IUIConfig _ViewConfig;
            private Action<T> _NewInitializer;
            private Func<ITierCoordinatorBuilder> _BuilderFactory;

            public IPersistedViewRoot Root { get; private set; }

            protected override Action<TEntity, TParentEntity> SetParent
            {
                get
                {
                    return _SetParent;
                }
            }

            protected override Func<TParentEntity, Expression<Func<TEntity, bool>>> ParentFilter
            {
                get
                {
                    return _ParentFilter;
                }
            }

            protected override ITypeMapper<T, TEntity> TypeMapper
            {
                get
                {
                    return _TypeMapper;
                }
            }

            protected override IUIConfig ViewConfig
            {
                get
                {
                    return _ViewConfig;
                }
            }

            internal Func<Action<T, TEntity>> MirroringBuilder { get; set; }

            internal ConstructedPersistedView(Func<ITierCoordinatorBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView owner, ITypeMapper<T, TEntity> typeMapper, Action<TEntity, TParentEntity> parentSetter, Func<TParentEntity, Expression<Func<TEntity, bool>>> parentFilter)
                : base(builderFactory, dataStoresManager, owner)
            {
                _TypeMapper = typeMapper;
                _BuilderFactory = builderFactory;
                _SetParent = parentSetter;
                _ParentFilter = parentFilter;
            }

            public IFinishedGraphFactory<TChildMirror, TChildEntity, TEntity, TDataStore> AddChild<TChildMirror, TChildEntity>()
                where TChildMirror : class, IEditableMirrorObject, new()
                where TChildEntity : class, new()
            {
                var childFactory = new FinishedGraphFactory<TChildMirror, TChildEntity, TEntity, TDataStore>();

                childFactory.Owner = this;
                childFactory.BuilderFactory = _BuilderFactory;
                childFactory.DataStoresManager = this.DataStoresManager;
                childFactory.Root = Root;

                return childFactory;
            }

            public IMembershipProviderFactory<TEntity, TChildEntity, TDataStore> AddMembershipChild<TChildEntity>()
                where TChildEntity : class, new()
            {
                return new MembershipProviderFactory<TDataStore>(
                    this,
                    _BuilderFactory,
                    this.DataStoresManager)
                    .ForEntities<TEntity, TChildEntity>();
            }

            protected override T ToLineItem(TEntity entity)
            {
                var lineItem = base.ToLineItem(entity);
                lineItem.SuppressNotifications = true;
                _Mirroring(lineItem, entity);
                lineItem.SuppressNotifications = false;

                return lineItem;
            }

            protected override void Load(IPersistedView owner)
            {
                _Mirroring = MirroringBuilder();
                base.Load(owner);

                if (_NewInitializer != null)
                {
                    this.Owner.DataNavigator.Creator = o => _NewInitializer((T)o);
                }
            }

            internal void SetViewConfig(IUIConfig config)
            {
                _ViewConfig = config;
            }

            internal void SetNewInitializer(Action<T> newInitializer)
            {
                _NewInitializer = newInitializer;
            }
        }

        private Action<ITypeMapper<T, TEntity>> _TypeMapperInitializer = null;
        private Func<Action<T, TEntity>> _MirroringBuilder = null;
        private Action<ICollectionViewWrapper> _ViewInitializer = null;
        private IUIConfig _ViewConfig = Nieko.Infrastructure.Navigation.RecordNavigation.UIConfig.NoView;
        private Action<T> _NewInitializer = null;

        internal IPersistedView Owner { get; set; }

        internal IPersistedViewRoot Root { get; set;}

        internal Func<ITierCoordinatorBuilder> BuilderFactory { get; set; }

        internal IDataStoresManager DataStoresManager { get; set; }

        public FinishedGraphFactory()
        {
            _TypeMapperInitializer = tm => tm.ImplyAll();
            _MirroringBuilder = () => (li, o) => { };
            _ViewInitializer = view => view.SortDescriptions.Add(new SortDescription(BindingHelper.Name((T li) => li.SourceKey), ListSortDirection.Ascending));
        }

        /// <summary>
        /// Specifies alternate setup of ModelView-Entity TypeMapper to that of
        /// implying all congruent properties on both types
        /// </summary>
        /// <param name="initializer">Initialization</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> InitializeTypeMapperBy(Action<ITypeMapper<T, TEntity>> initializer)
        {
            _TypeMapperInitializer = initializer;

            return this;
        }

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
        public IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> BuildMirroringBy(Func<Action<T, TEntity>> mirroringBuilder)
        {
            _MirroringBuilder = mirroringBuilder;

            return this;
        }
       
        /// <summary>
        /// Specifies additional setup actions for ListCollectionView of this tier
        /// </summary>
        /// <param name="initializer">Additional actions</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> InitializingViewBy(Action<ICollectionViewWrapper> initializer)
        {
            _ViewInitializer = initializer;

            return this;
        }

        /// <summary>
        /// Provides Navigation UI Control configuration. The default UI configuration
        /// is to not show a Navigation Control
        /// </summary>
        /// <param name="viewConfig">Configuration</param>
        /// <returns>Current Instance</returns>
        public IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> WithUIConfig(IUIConfig viewConfig)
        {
            _ViewConfig = viewConfig;

            return this;
        }

        /// <summary>
        /// Provides additional actions to perform when a new ModelView
        /// is added (i.e. a line item representing a new entity instance)
        /// </summary>
        /// <param name="initializer">Initialization action</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TParentEntity, TDataStore> InitializingNewItemBy(Action<T> initializer)
        {
            _NewInitializer = initializer;

            return this;
        }

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
        public IConstructedPersistedView<T, TEntity, TDataStore> Build(Action<TEntity, TParentEntity> parentSetter, Func<TParentEntity, Expression<Func<TEntity, bool>>> parentFilter)
        {
            return Build(parentSetter, parentFilter, true);
        }

        public IConstructedPersistedView<T, TEntity, TDataStore> Build(Action<TEntity, TParentEntity> parentSetter, Func<TParentEntity, Expression<Func<TEntity, bool>>> parentFilter, bool cascadeDelete)
        {
            var typeMapper = TypeMapper.New<T, TEntity>();
            ConstructedPersistedView result;

            _TypeMapperInitializer(typeMapper);
            result = new ConstructedPersistedView(BuilderFactory, DataStoresManager, Owner, typeMapper, parentSetter, parentFilter);
            result.CascadeDelete = cascadeDelete;
            result.MirroringBuilder = _MirroringBuilder;

            result.SetViewConfig(_ViewConfig);

            if (_ViewInitializer != null)
            {
                SetViewInitializer(result);
            }

            if (_NewInitializer != null)
            {
                result.SetNewInitializer(_NewInitializer);
            }

            return result;
        }

        private void SetViewInitializer(ConstructedPersistedView persistedView)
        {
            EventHandler viewChangedHandler = null;
            EventHandler disposingHandler = null;

            viewChangedHandler = (sender, args) =>
            {
                _ViewInitializer(persistedView);
            };

            persistedView.ViewChanged += viewChangedHandler;

            disposingHandler = (sender, args) =>
            {
                persistedView.ViewChanged -= viewChangedHandler;
                persistedView.Disposing -= disposingHandler;
            };

            persistedView.Disposing += disposingHandler;
        }
    }
}
