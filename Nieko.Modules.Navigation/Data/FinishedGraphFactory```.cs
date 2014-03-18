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
    /// Final fluent method set for a IGraphFactory[Owner] building all objects in 
    /// one-to-many tier (i.e. top-most ModelView tier)
    /// </summary>
    /// <typeparam name="T">ModelView</typeparam>
    /// <typeparam name="Owner">Parent in one-to-many relationship</typeparam>
    /// <typeparam name="TEntity">Mirrored entity</typeparam>
    /// <typeparam name="TDataStore">Store</typeparam>
    internal class FinishedGraphFactory<T, TEntity, TDataStore> : IFinishedGraphFactory<T, TEntity, TDataStore>
        where T : class, IEditableMirrorObject, new()
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        /// <summary>
        /// Internal implementation of IPersistedView, built by containing FinishedGraphFactory class.
        /// Provides fluent method for creating child tiers
        /// </summary>
        /// <remarks>
        /// Use of nested class allows for simplification of generic type arguments and internal IPersistedView
        /// construction without sacrificing strong-typing and simplicity in Fluent Factory methods.
        /// </remarks>
        public class ConstructedPersistedView : PersistedView<T, TEntity, TDataStore>, IConstructedPersistedView<T, TEntity, TDataStore>
        {
            private ITypeMapper<T, TEntity> _TypeMapper;
            private Action<T, TEntity> _Mirroring = null;
            private IUIConfig _ViewConfig;
            private Func<IQueryable<TEntity>, IQueryable<TEntity>> _EntitiesQuery;
            private Func<IEnumerable<T>, IEnumerable<T>> _ItemsQuery;
            private Action<T> _NewInitializer;
            private Func<IDataNavigatorOwnerBuilder> _BuilderFactory;

            protected override ITypeMapper<T, TEntity> Mapper
            {
                get
                {
                    return _TypeMapper;
                }
            }

            public IPersistedViewRoot Root { get; private set; }

            internal Func<Action<T, TEntity>> MirroringBuilder { get; set; }

            protected override Func<IQueryable<TEntity>, IQueryable<TEntity>> EntitiesQuery
            {
                get
                {
                    return _EntitiesQuery == null ? base.EntitiesQuery : _EntitiesQuery;
                }
            }

            protected override Func<IEnumerable<T>, IEnumerable<T>> ItemsQuery
            {
                get
                {
                    return _ItemsQuery == null ? base.ItemsQuery : _ItemsQuery;
                }
            }

            protected override IUIConfig ViewConfig
            {
                get
                {
                    return _ViewConfig;
                }
            }

            internal ConstructedPersistedView(Func<IDataNavigatorOwnerBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedViewRoot root, ITypeMapper<T, TEntity> typeMapper)
                : base(builderFactory, dataStoresManager, root)
            {
                _TypeMapper = typeMapper;
                _BuilderFactory = builderFactory;
                Root = root;

                _ViewConfig = new MainUIConfig(this);
            }

            /// <summary>
            /// Starts fluent factory for adding a child tier below this one
            /// </summary>
            /// <typeparam name="TChildMirror">ModelView for lower tier</typeparam>
            /// <typeparam name="TChildEntity">Mirrored Entity for lower tier</typeparam>
            /// <returns>Factory for building lower tier</returns>
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

            internal void SetEntitiesQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
            {
                _EntitiesQuery = query;
            }

            internal void SetItemsQuery(Func<IEnumerable<T>, IEnumerable<T>> query)
            {
                _ItemsQuery = query;
            }

            internal void SetViewConfig(IUIConfig config)
            {
                _ViewConfig = config;
            }

            internal void SetNewInitializer(Action<T> newInitializer)
            {
                _NewInitializer = newInitializer;
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

            protected override T ToLineItem(TEntity entity)
            {
                var lineItem = base.ToLineItem(entity);
                _Mirroring(lineItem, entity);

                return lineItem;
            }
        }

        private Action<ITypeMapper<T, TEntity>> _TypeMapperInitializer = null;
        private Func<Action<T, TEntity>> _MirroringBuilder = null;
        private Action<ListCollectionView> _ViewInitializer = null;
        private Func<IQueryable<TEntity>, IQueryable<TEntity>> _EntityQuery = null;
        private Func<IEnumerable<T>, IEnumerable<T>> _ItemsQuery = null;
        private IUIConfig _ViewConfig = null;
        private Action<T> _NewInitializer = null;

        internal IPersistedViewRoot Root { get; set; }

        internal Func<IDataNavigatorOwnerBuilder> BuilderFactory { get; set; }

        internal IDataStoresManager DataStoresManager { get; set; }

        internal FinishedGraphFactory()
        {
            // By default map all congruent properties between ModelView and Entity
            _TypeMapperInitializer = tm => tm.ImplyAll();
            // By default, no additional actions required for mirroring
            _MirroringBuilder = () => (li, o) => { };
            // By default, sort by the PrimaryKey property of the mirrored Entity
            _ViewInitializer = view => view.SortDescriptions.Add(new SortDescription(BindingHelper.Name((T li) => li.SourceKey), ListSortDirection.Ascending));
        }

        /// <summary>
        /// Specifies alternate setup of ModelView-Entity TypeMapper to that of
        /// implying all congruent properties on both types
        /// </summary>
        /// <param name="initializer">Initialization</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TDataStore> InitializeTypeMapperBy(Action<ITypeMapper<T, TEntity>> initializer)
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
        public IFinishedGraphFactory<T, TEntity, TDataStore> BuildMirroringBy(Func<Action<T, TEntity>> mirroringBuilder)
        {
            _MirroringBuilder = mirroringBuilder;

            return this;
        }

        /// <summary>
        /// Specifies additional setup actions for ListCollectionView of this tier
        /// </summary>
        /// <param name="initializer">Additional actions</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TDataStore> InitializingViewBy(Action<ListCollectionView> initializer)
        {
            _ViewInitializer = initializer;

            return this;
        }

        /// <summary>
        /// Provides alternative Entity Expression builder to retrieve Entities with
        /// </summary>
        /// <param name="entityQuery">Expression builder</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TDataStore> WithEntityQuery(Func<IQueryable<TEntity>, IQueryable<TEntity>> entityQuery)
        {
            _EntityQuery = entityQuery;

            return this;
        }

        /// <summary>
        /// Provides post-mirroring filtering of ModelView line-items
        /// </summary>
        /// <param name="itemsQuery">Line item filter</param>
        /// <returns>Current instance</returns>
        public IFinishedGraphFactory<T, TEntity, TDataStore> WithItemsQuery(Func<IEnumerable<T>, IEnumerable<T>> itemsQuery)
        {
            _ItemsQuery = itemsQuery;

            return this;
        }

        /// <summary>
        /// Provides Navigation UI Control configuration
        /// </summary>
        /// <param name="viewConfig">Configuration</param>
        /// <returns>Current Instance</returns>
        public IFinishedGraphFactory<T, TEntity, TDataStore> WithUIConfig(IUIConfig viewConfig)
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
        public IFinishedGraphFactory<T, TEntity, TDataStore> InitializingNewItemBy(Action<T> initializer)
        {
            _NewInitializer = initializer;

            return this;
        }

        /// <summary>
        /// Build IPersistedView using parameters provided to this factory
        /// </summary>
        /// <returns>IPersistedView for this tier</returns>
        public IConstructedPersistedView<T, TEntity, TDataStore> Build()
        {
            var typeMapper = TypeMapper.New<T, TEntity>();
            ConstructedPersistedView result;

            _TypeMapperInitializer(typeMapper);
            result = new ConstructedPersistedView(BuilderFactory, DataStoresManager, Root, typeMapper);
            result.MirroringBuilder = _MirroringBuilder;
            result.SetEntitiesQuery(_EntityQuery);
            result.SetItemsQuery(_ItemsQuery);
            if (_ViewConfig != null)
            {
                result.SetViewConfig(_ViewConfig);
            }

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
            PropertyChangedEventHandler propertyChangedHandler = null;
            EventHandler disposingHandler = null;

            propertyChangedHandler = (sender, args) =>
            {
                if (args.PropertyName == BindingHelper.Name((IPersistedView v) => v.View))
                {
                    _ViewInitializer(persistedView.View);
                }
            };

            persistedView.PropertyChanged += propertyChangedHandler;

            disposingHandler = (sender, args) =>
            {
                persistedView.PropertyChanged -= propertyChangedHandler;
                persistedView.Disposing -= disposingHandler;
            };

            persistedView.Disposing += disposingHandler;
        }
    }
}
