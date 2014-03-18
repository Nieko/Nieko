using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Data;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Nieko.Infrastructure;
using Nieko.Infrastructure.ComponentModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Nieko.Infrastructure.Collections;
using System.Linq.Expressions;
using Nieko.Infrastructure.Windows.Data;
using n = Nieko.Infrastructure.Navigation;

namespace Nieko.Modules.Navigation.Data
{
    internal abstract class PersistedView<T> : ListCollectionViewWrapper, IPersistedView<T>
        where T : class, IEditableMirrorObject, new()
    {
        private static object _Lock = new object();
        private bool _IsDisposing = false;
        private Dictionary<Type, object> _TypePreloadActions = new Dictionary<Type, object>();

        private Func<IDataNavigatorOwnerBuilder> _BuilderFactory;
        private IWeakEventRouter _ViewCurrentChangedRouter;
        private IWeakEventRouter _CollectionChangedRouter;

        private ObservableCollection<T> _Items;
        private ListCollectionView _View;
        private Dictionary<PrimaryKey, T> _DeletedItems = new Dictionary<PrimaryKey,T>();

        public event EventHandler Loaded;
        public event EventHandler Disposing;
        public event PropertyChangedEventHandler PropertyChanged;

        protected IDataStoresManager DataStoresManager { get; private set; }

        public ReadOnlyDictionary<PrimaryKey> DeletedItems { get; private set; }

        protected Action<TEntity> GetKeyPreload<TEntity>()
        {
            object item;
            Action<TEntity> preload;
            IList<Action<TEntity>> preloadActions;

            lock (_Lock)
            {
                if (!_TypePreloadActions.TryGetValue(typeof(TEntity), out item))
                {
                    preloadActions = new List<Action<TEntity>>();

                    var properties = PrimaryKey.GetKeyProperties(typeof(TEntity))
                        .Select(kp => typeof(TEntity).GetProperty(kp.Key))
                        .Where(p => typeof(IPrimaryKeyed).IsAssignableFrom(p.PropertyType));
                    var parameter = Expression.Parameter(typeof(TEntity), "o");

                    foreach (var property in properties)
                    {
                        var accessor = Expression.Lambda<Func<TEntity, object>>(
                            Expression.Convert(
                                Expression.PropertyOrField(
                                    parameter,
                                    property.Name),
                                    typeof(object)),
                            parameter)
                            .Compile();
                        preloadActions.Add(o => { var dummy = accessor(o); });
                    }

                    preload = o =>
                    {
                        foreach (var action in preloadActions)
                        {
                            action(o);
                        }
                    };

                    _TypePreloadActions.Add(typeof(TEntity), preload);
                }
                else
                {
                    preload = (Action<TEntity>)item;
                }
            }

            return preload;
        }

        protected virtual IUIConfig ViewConfig 
        {
            get
            {
                return n.RecordNavigation.UIConfig.NoView;
            }
        }

        public ObservableCollection<T> Items 
        {
            get
            {
                return _Items;
            }
            set
            {
                if (_Items == value)
                {
                    return;
                }

                if (_Items != null)
                {
                    _CollectionChangedRouter.CancelSubscription();
                }

                _Items = value;

                if (_Items != null)
                {
                    _CollectionChangedRouter = WeakEventRouter.CreateInstance(this, Items,
                        () => default(NotifyCollectionChangedEventArgs),
                        (s, d) => s.CollectionChanged += d.Handler,
                        (s, d) => s.CollectionChanged -= d.Handler,
                        ItemsChanged);
                }
            }
        }

        public override ListCollectionView View 
        {
            get
            {
                return _View;
            }
            set
            {
                if (_View == value)
                {
                    return;
                }

                _View = value;

                RaisePropertyChanged(BindingHelper.Name(() => View)); 
            }
        }

        public IDataNavigatorOwner Owner { get; private set; }

        T IPersistedView<T>.CurrentItem
        {
            get 
            {
                return View == null ? null : (T)View.CurrentItem; 
            }
        }

        int IPersistedView.CurrentPosition
        {
            get
            {
                return Owner == null || Owner.DataNavigator == null ? -1 : Owner.DataNavigator.CurrentPosition -1;
            }
            set
            {
                Owner.DataNavigator.CurrentPosition = value + 1;
            }
        }

        public PersistedView(Func<IDataNavigatorOwnerBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView parent)
        {
            DeletedItems = ReadOnlyDictionary<PrimaryKey>.Create(_DeletedItems);

            _BuilderFactory = builderFactory;
            DataStoresManager = dataStoresManager;

            EventHandler ownerLoaded = null;

            ownerLoaded = (sender, args) =>
            {
                Load(parent);

                var handler = Loaded;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                parent.Loaded -= ownerLoaded;
            };

            parent.Loaded += ownerLoaded;
        }

        protected virtual void Load(IPersistedView parent)
        {
            var builder = _BuilderFactory().CreateDataNavigator(parent)
                .UsingPersistedView<T>(this)
                .WithParent(parent.Owner);

            if (ViewConfig.CreateView)
            {
                builder = builder.ProvidingUIControlAt(ViewConfig);
            }

            Owner = builder.Build();

            _ViewCurrentChangedRouter = WeakEventRouter.CreateInstance(this,
                Owner.DataNavigator,
                () => default(EventArgs),
                (p, d) => p.ViewCurrentChanged += d.Handler,
                (p, d) => p.ViewCurrentChanged -= d.Handler,
                ViewCurrentChanged);
        }

        public abstract void RunOpened(Action storeOpenedAction);

        public abstract IEnumerable<T> ItemsLoader();

        protected abstract PrimaryKey GetSourceKey<TSource>(TSource entity);

        protected IEnumerable<T> LoadFromEntities<TDataStore, TEntity>(Action<TDataStore> resolver, Func<IDataStore, IEnumerable<TEntity>> dataLoader, Func<TEntity, T> converter)
            where TDataStore : class, IDataStore
        {
            IList<T> result = null;

            DataStoresManager.DoUnitOfWork<TDataStore>((dataStore) =>
                {
                    result = dataLoader(dataStore)
                        .Select(o => 
                            {
                                var lineItem = converter(o);
                                lineItem.SourceKey = GetSourceKey(o);

                                return lineItem;
                            })
                        .ToList();
                });

            return result;
        }

        public virtual void PersistChanges(IPersistedView owner)
        {
            foreach (var item in _DeletedItems.Values)
            {
                Delete(owner, item);
            }
            _DeletedItems.Clear();

            foreach (var item in Items
                .Where(o => o.SourceKey == null ||
                    o.HasChanged))
            {
                Save(owner, item);
            }
        }

        protected abstract void Save(IPersistedView parent, T item);
        protected abstract void Delete(IPersistedView parent, T item);

        public void Dispose()
        {
            if(_IsDisposing)
            {
                return;
            }

            var handler = Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            DisposeImpl();
        }

        protected virtual void DisposeImpl()
        {
            Items.Clear();
        }

        private void ViewCurrentChanged(PersistedView<T> owner, IDataNavigator sender, EventArgs args)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                RaisePropertyChanged(BindingHelper.Name(()=> CurrentItem));
                RaisePropertyChanged(BindingHelper.Name(() => CurrentPosition));
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ItemsChanged(PersistedView<T> owner, INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in args.OldItems
                    .Cast<T>()
                    .Where(o => (o.SourceKey != null) 
                        && !_DeletedItems.ContainsKey(o.SourceKey)))
                {
                    _DeletedItems.Add(item.SourceKey, item);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                _DeletedItems.Clear();
            }
        }
    }

    internal abstract class PersistedView<T, TEntity, TDataStore> : PersistedView<T>
        where T : class, IEditableMirrorObject, new()
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> EntitiesQuery
        {
            get
            {
                return query => query;
            }
        }

        protected virtual Func<IEnumerable<T>, IEnumerable<T>> ItemsQuery
        {
            get
            {
                return items => items;
            }
        }

        protected abstract ITypeMapper<T, TEntity> Mapper { get; }

        public PersistedView(Func<IDataNavigatorOwnerBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView owner)
            : base(builderFactory, dataStoresManager, owner) { }

        protected virtual T ToLineItem(TEntity entity)
        {
            var lineItem = new T();

            Mapper.From(lineItem, entity);
            lineItem.SourceKey = GetSourceKey(entity);
            GetKeyPreload<TEntity>()(entity);

            return lineItem;
        }

        public override void RunOpened(Action storeOpenedAction)
        {
            DataStoresManager.GetDataStore<TDataStore>(this);
            storeOpenedAction();
            DataStoresManager.ReleaseDataStore<TDataStore>(this);
        }

        public override IEnumerable<T> ItemsLoader()
        {
            IEnumerable<T> items = null;

            DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
            {
                items = EntitiesQuery(dataStore.GetItems<TEntity>())
                    .ToList()
                    .Select(o => ToLineItem(o));

                items = ItemsQuery(items);
            });

            return items;
        }

        protected override PrimaryKey GetSourceKey<TSource>(TSource entity)
        {
            return (entity as IPrimaryKeyed).PrimaryKey;
        }

        protected override void Save(IPersistedView parent, T item)
        {
            if (item.SourceKey == null)
            {
                DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    TEntity entity = new TEntity();
                    Mapper.To(item, entity);

                    dataStore.Save<TEntity>(entity);

                    item.SourceKey = GetSourceKey(entity);
                });
            }
            else
            {
                DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    TEntity entity = dataStore.GetItem<TEntity>(item.SourceKey.ToFilterExpression<TEntity>());
                    Mapper.To(item, entity);

                    dataStore.Save<TEntity>(entity);
                });
            }
        }

        protected override void Delete(IPersistedView parent, T item)
        {
            DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
            {
                TEntity entity = dataStore.GetItem<TEntity>(item.SourceKey.ToFilterExpression<TEntity>());

                dataStore.Delete<TEntity>(entity);
            });
        }
    }

    internal abstract class PersistedView<T, TEntity, TParentEntity, TDataStore> : PersistedView<T>
        where T : class, IEditableMirrorObject, new()
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        public bool CascadeDelete { get; set; }

        protected virtual Action<TEntity, TParentEntity> SetParent
        {
            get
            {
                return (entity, parentEntity) => { };
            }
        }

        protected virtual Func<TParentEntity, Expression<Func<TEntity, bool>>> ParentFilter
        {
            get
            {
                return (parentEntity) => (entity) => true;
            }
        }

        protected abstract ITypeMapper<T, TEntity> TypeMapper { get; }

        public PersistedView(Func<IDataNavigatorOwnerBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView owner)
            : base(builderFactory, dataStoresManager, owner) 
        {
            CascadeDelete = true;
        }

        protected override PrimaryKey GetSourceKey<TSource>(TSource entity)
        {
            return (entity as IPrimaryKeyed).PrimaryKey;
        }

        public override void RunOpened(Action storeOpenedAction)
        {
            DataStoresManager.GetDataStore<TDataStore>(this);
            storeOpenedAction();
            DataStoresManager.ReleaseDataStore<TDataStore>(this);
        }

        public override IEnumerable<T> ItemsLoader()
        {
            IEnumerable<T> items = null;

            DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    IEditableMirrorObject parentCurrentItem = (IEditableMirrorObject)Owner.Parent.PersistedView.View.CurrentItem;
                    if (parentCurrentItem == null || parentCurrentItem.SourceKey == null)
                    {
                        items = new List<T>();
                        return;
                    }

                    TParentEntity parentEntity = dataStore.GetItem<TParentEntity>(parentCurrentItem.SourceKey.ToFilterExpression<TParentEntity>());
                    if (parentEntity == null)
                    {
                        items = new List<T>();
                        return;
                    }
 
                    items = dataStore.GetItems<TEntity>().Where(this.ParentFilter(parentEntity))
                        .ToList()
                        .Select(o => ToLineItem(o)); 
                });

            return items;
        }

        protected virtual T ToLineItem(TEntity entity)
        {
            var lineItem = new T();

            TypeMapper.From(lineItem, entity);
            lineItem.SourceKey = GetSourceKey(entity);

            return lineItem;
        }

        public override void PersistChanges(IPersistedView owner)
        {
            if (owner.DeletedItems.Any())
            {
                DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    TParentEntity deletedParent;

                    foreach (var item in owner.DeletedItems)
                    {
                        deletedParent = dataStore.GetItem<TParentEntity>(item.Key.ToFilterExpression<TParentEntity>());
                        if (deletedParent != null)
                        {
                            foreach (var child in dataStore.GetItems<TEntity>()
                                .Where(ParentFilter(deletedParent))
                                .ToList())
                            {
                                if (CascadeDelete)
                                {
                                    dataStore.Delete(child);
                                }
                                else
                                {
                                    SetParent(child, default(TParentEntity));
                                }
                            }
                        }
                    }
                });
            }
            
            if(owner.View.CurrentItem != null)
            {
                base.PersistChanges(owner);
            }
        }

        protected override void Save(IPersistedView parent, T item)
        {
            IEditableMirrorObject parentCurrentItem = (IEditableMirrorObject)Owner.Parent.PersistedView.View.CurrentItem;

            if (parentCurrentItem == null)
            {
                return;
            }

            DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    TParentEntity parentEntity = default(TParentEntity);
                    parentEntity = dataStore.GetItem<TParentEntity>(parentCurrentItem.SourceKey.ToFilterExpression<TParentEntity>());

                    if (item.SourceKey == null)
                    {
                        TEntity entity = new TEntity();
                        TypeMapper.To(item, entity);
                        SetParent(entity, parentEntity);

                        dataStore.Save<TEntity>(entity);

                    }
                    else
                    {
                        TEntity entity = dataStore.GetItem<TEntity>(item.SourceKey.ToFilterExpression<TEntity>());
                        TypeMapper.To(item, entity);
                        SetParent(entity, parentEntity);

                        dataStore.Save<TEntity>(entity);
                    }
                });
        }

        protected override void Delete(IPersistedView parent, T item)
        {
            DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    TEntity entity = dataStore.GetItem<TEntity>(item.SourceKey.ToFilterExpression<TEntity>());

                    dataStore.Delete<TEntity>(entity); 
                });
        }
    }
}
