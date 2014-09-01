using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure.Windows;

namespace Nieko.Modules.Navigation.Data
{
    internal abstract class MembershipProvider<T, TEntity, TParentEntity, TDataStore> : PersistedView<T, TEntity, TParentEntity, TDataStore>, IMembershipProvider<T>
        where T : class, IMembershipProviderLineItem, new()
        where TDataStore : class, IDataStore
        where TEntity : class, new()
    {
        private NotifyingFields _Fields;
        private IWeakEventRouter _ItemsCollectionChangedRouter;
        private Func<IEnumerable<T>> _AllPossibleMemberships;
        private Func<IEnumerable<T>> _CurrentMemberships;

        public event EventHandler Reset;

        public ObservableCollection<T> AvailableMemberships { get; private set; }

        public T NewItem
        {
            get
            {
                return _Fields.Get(()=>NewItem);
            }
            set
            {
                if (NewItem == value)
                {
                    return;
                }
                _Fields.Set(() => NewItem, value);
                AddEnabled = value != null;
            }
        }

        public ICommand AddCommand { get; private set; }
       
        public bool AddEnabled 
        {
            get
            {
                return _Fields.Get(() => AddEnabled);
            }
            private set
            {
                _Fields.Set(() => AddEnabled, value);
            }
        }
        
        public ICommand RemoveCommand { get; private set; }

        public virtual Func<IMembershipProvider<T>, bool> RemoveAllowed { get; set; }

        protected virtual Func<IPersistedView, IEnumerable<T>> AllPossibleMemberships 
        {
            get
            {
                return owner =>
                    {
                        IEnumerable<T> allPossibilities = null;

                        DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                            {
                                allPossibilities = dataStore.GetItems<TEntity>()
                                    .ToList()
                                    .Select(o => ToLineItem(o))
                                    .OrderBy(l => l.SourceKey);
                            });

                        return allPossibilities;
                    };
            }
        }

        protected abstract MembershipPersistence<TParentEntity, TEntity> MembershipPersistence { get; }

        protected virtual Func<IPersistedView, PrimaryKey> ParentEntityPrimaryKey 
        {
            get
            {
                return (owner) => 
                    {
                        if(owner == null || owner.CurrentItem == null)
                        {
                            return null;
                        }
                        else
                        {
                            return (owner.CurrentItem as IEditableMirrorObject).SourceKey;
                        }
                    };
            }
        }

        protected virtual Func<IPersistedView, IEnumerable<T>> CurrentMemberships 
        {
            get
            {
                return owner =>
                    {
                        PrimaryKey parentPrimaryKey = ParentEntityPrimaryKey(owner);
                        List<TEntity> memberships = null;

                        DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                            {
                                var exists = parentPrimaryKey != null && dataStore.GetItems<TParentEntity>().Any(parentPrimaryKey.ToFilterExpression<TParentEntity>());

                                if (!exists)
                                {
                                    memberships = new List<TEntity>();

                                }
                                else
                                {
                                    memberships = dataStore.GetItems<TParentEntity>().Where(parentPrimaryKey.ToFilterExpression<TParentEntity>())
                                        .SelectMany(MembershipPersistence.Memberships)
                                        .ToList();
                                }
                            });

                        return memberships
                            .Select(o => ToLineItem(o));
                    };
            }
        }

        protected virtual Func<IPersistedView, bool> NoCurrentOwner
        {
            get
            {
                return parent => parent == null || parent.CurrentItem == null;
            }
        }

        public MembershipProvider(Func<ITierCoordinatorBuilder> builderFactory, IDataStoresManager dataStoresManager, IPersistedView owner)
            : base(builderFactory, dataStoresManager, owner)
        {
            _Fields = new NotifyingFields(this, RaisePropertyChanged);

            AvailableMemberships = new ObservableCollection<T>();
            AddCommand = new RelayCommand(AddLineItem);
            AddEnabled = false;
            RemoveCommand = new RelayCommand(RemoveLineItem);
            RemoveAllowed = mp => mp.Items.Count > 1;
        }

        protected override void Load(IPersistedView owner)
        {
            ChangeSubscriptions(true);

            _AllPossibleMemberships = () => AllPossibleMemberships(owner);
            _CurrentMemberships = () => CurrentMemberships(owner);

            base.Load(owner);

            ChangeSubscriptions(false);

            var handler = Reset;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

        }

        public override IEnumerable<T> ItemsLoader()
        {
            UpdateAvailableMemberships(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return _CurrentMemberships();
        }

        protected override void DisposeImpl()
        {
            ChangeSubscriptions(true);
            base.DisposeImpl();
        }

        private void ChangeSubscriptions(bool remove)
        {
            if (remove)
            {
                if (_ItemsCollectionChangedRouter != null)
                {
                    _ItemsCollectionChangedRouter.CancelSubscription();
                    _ItemsCollectionChangedRouter = null;
                }
            }
            else
            {
                if (Owner != null)
                {
                    _ItemsCollectionChangedRouter = WeakEventRouter.CreateInstance(this, Owner,
                        () => default(NotifyCollectionChangedEventArgs),
                        (p, d) => p.ItemsCollectionChanged += d.Handler,
                        (p, d) => p.ItemsCollectionChanged -= d.Handler,
                        (o, sender, args) => UpdateAvailableMemberships(args));
                }
            }
        }

        private void RemoveLineItem(object parameters)
        {
            View.Remove(parameters);
        }

        private void AddLineItem(object parameters)
        {
            if (NewItem == null)
            {
                AddEnabled = false;
                return;
            }

            Items.Add(NewItem);
        }

        private void UpdateAvailableMemberships(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems
                        .Cast<T>()
                        .Intersect(AvailableMemberships)
                        .ToList())
                    {
                        AvailableMemberships.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems
                        .Cast<T>()
                        .Except(AvailableMemberships)
                        .ToList())
                    {
                        AvailableMemberships.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    AvailableMemberships.Clear();
                    if (_AllPossibleMemberships != null && _CurrentMemberships != null)
                    {
                        foreach (var membership in _AllPossibleMemberships()
                                .Except(_CurrentMemberships()))
                        {
                            AvailableMemberships.Add(membership);
                        }
                    }
                    break;

            }

            SetRemoveVisibilities();
        }

        private void SetRemoveVisibilities()
        {
            var visibility = RemoveAllowed(this) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            foreach (var lineItem in Items)
            {
                lineItem.RemoveVisibility = visibility;
            }
        }

        #region IMembershipProvider Implementation
        object IMembershipProvider.NewItem
        {
            get { return this.NewItem; }
            set { this.NewItem = (T)value; }
        }

        object IMembershipProvider.Options
        {
            get { return this.AvailableMemberships; }
        }
        #endregion

        public override void PersistChanges(IPersistedView owner)
        {
            PrimaryKey ownerEntityPrimaryKey = ParentEntityPrimaryKey(owner);

            DataStoresManager.DoUnitOfWork<TDataStore>(dataStore =>
                {
                    TParentEntity parentEntity = default(TParentEntity);
                    TParentEntity deletedEntity;
                    if (ownerEntityPrimaryKey != null)
                    {
                        parentEntity = dataStore.GetItem<TParentEntity>(ownerEntityPrimaryKey.ToFilterExpression<TParentEntity>());
                    }

                    foreach (var deleted in owner.DeletedItems)
                    {
                        if (ownerEntityPrimaryKey == deleted.Key)
                        {
                            parentEntity = default(TParentEntity);
                        }
                        deletedEntity = dataStore.GetItem<TParentEntity>(deleted.Key.ToFilterExpression<TParentEntity>());
                        MembershipPersistence.Clear(deletedEntity);
                        dataStore.Save<TParentEntity>(deletedEntity);
                    }


                    if (parentEntity != null)
                    {
                        MembershipPersistence.Clear(parentEntity);
                        
                        foreach (var member in Items
                            .Select(lineItem => dataStore.GetItem<TEntity>(lineItem.SourceKey.ToFilterExpression<TEntity>()))
                            .Where(o => o != null))
                        {
                            MembershipPersistence.Add(parentEntity, member);
                        }

                        dataStore.Save<TParentEntity>(parentEntity);
                    }
                    
                });
        }

        protected override void Save(IPersistedView parent, T item) { } // Ignore individual persistence requests

        protected override void Delete(IPersistedView parent, T item) { } // Ignore individual persistence requests
    }
}