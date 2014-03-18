using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using System.ComponentModel;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;
using Nieko.Infrastructure.Windows.Data;
using System.Xml.Serialization;
using System.IO;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure;
using Nieko.Infrastructure.Navigation;
using System.Transactions;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    /// <summary>
    /// Generic implementation of an <seealso cref="IDataNavigatorOwner"/>
    /// </summary>
    /// <typeparam name="T">Type of entity mirror in collection at this tier</typeparam>
    internal class DataNavigatorOwner<T> : IDataNavigatorOwner
        where T : IEditableMirrorObject
    {
        private object _Lock = new object();
        private bool _IgnoreEditStateEvents;
        private bool _IsDisposing;
        private NotifyingFields _Fields;
        private HashSet<IDataNavigatorOwner> _Children;
        private Dictionary<IDataNavigatorOwner, Action> _RemoveActionByChild;
        private IPersistedView<T> _PersistedView;
        
        private Action _PendingPersistAction = null;
        
        private IWeakEventRouter _ParentCurrentChangedRouter;
        private IWeakEventRouter _ParentDataNavigatorPropertyChangedRouter;
        private IWeakEventRouter _ParentPropertyChangedRouter;
        private IWeakEventRouter _DataNavigatorPropertyChangedRouter;
        private IWeakEventRouter _ItemsCollectionChangedRouter;
        private IWeakEventRouter _ViewCurrentChanging;
        private IWeakEventRouter _ParentPersistingRouter;

        public event EventHandler Disposing = delegate { };
        public event NotifyCollectionChangedEventHandler ItemsCollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<PersistingChangesEventArgs> PersistingChanges = delegate { };

        public bool AllowEdit 
        {
            get
            {
                return _Fields.Get(() => AllowEdit);
            }
            private set
            {
                _Fields.Set(() => AllowEdit, value); 
            }
        }

        public IPersistedView PersistedView
        {
            get
            {
                return _PersistedView;
            }
        }

        public IDataNavigatorOwner Parent 
        {
            get
            {
                return _Fields.Get(() => Parent);
            }
            private set
            {
                if (Parent == value)
                {
                    return;
                }

                if (Parent != null)
                {
                    _ParentCurrentChangedRouter.CancelSubscription();
                    _ParentDataNavigatorPropertyChangedRouter.CancelSubscription();
                    _ParentPropertyChangedRouter.CancelSubscription();
                    _ParentPersistingRouter.CancelSubscription();
                    Parent.Hierarchy.RemoveChild(this);
                }

                _Fields.Set(() => Parent, value);
                if (value != null)
                {
                    _ParentCurrentChangedRouter = WeakEventRouter.CreateInstance(this, Parent.DataNavigator,
                        () => default(EventArgs),
                        (p, d) => p.ViewCurrentChanged += d.Handler,
                        (p, d) => p.ViewCurrentChanged -= d.Handler,
                        (s, p, a) => ParentDataNavigatorCurrentChanged(p, a));

                    _ParentDataNavigatorPropertyChangedRouter = WeakEventRouter.CreateInstance(this, Parent.DataNavigator,
                        () => default(PropertyChangedEventArgs),
                        (p, d) => p.PropertyChanged += d.Handler,
                        (p, d) => p.PropertyChanged -= d.Handler,
                        (s, p, a) => ParentDataNavigatorPropertyChanged(p, a));

                    _ParentPropertyChangedRouter = WeakEventRouter.CreateInstance(this, Parent,
                        () => default(PropertyChangedEventArgs),
                        (p, d) => p.PropertyChanged += d.Handler,
                        (p, d) => p.PropertyChanged -= d.Handler,
                        (s, p, a) => ParentPropertyChanged(p, a));

                    _ParentPersistingRouter = WeakEventRouter.CreateInstance(this, Parent,
                        () => default(PersistingChangesEventArgs),
                        (p, d) => p.PersistingChanges += d.Handler,
                        (p, d) => p.PersistingChanges -= d.Handler,
                        (s, p, a) => PersistChanges(p.PersistedView)); 

                    value.Hierarchy.AddChild(this);
                }
            }
        }

        public IDataNavigator DataNavigator 
        {
            get
            {
                return _Fields.Get(() => DataNavigator); 
            }
            private set
            {
                _Fields.Set(() => DataNavigator, value);
            }
        }

        public ObservableCollection<T> Items 
        {
            get
            {
                return _PersistedView.Items;
            }
            private set
            {
                _PersistedView.Items = value;
            }
        }

        public IOwnershipHierarchy Hierarchy { get; private set; }

        protected IDataStoresManager DataStoresManager { get; private set; }

        private DataNavigatorOwner(IDataNavigatorOwner parent, INotifyDisposing owner, IDataStoresManager dataStoresManager, IDataNavigator dataNavigator, IPersistedView<T> persistedView, IViewNavigator regionNavigator)
        {
            DataStoresManager = dataStoresManager;
            _PersistedView = persistedView;

            _Fields = new NotifyingFields(this, () => PropertyChanged);
            _Children = new HashSet<IDataNavigatorOwner>();
            _RemoveActionByChild = new Dictionary<IDataNavigatorOwner, Action>();
            _IgnoreEditStateEvents = false;
            Hierarchy = new OwnershipHierarchy(_Children)
            {
                ChildAddition = AddChild,
                ChildRemoval = RemoveChild,
                Parent = this
            };

            EventHandler ownerDisposing = null;
            EventHandler persistedViewDisposing = null;

            AllowEdit = parent == null;

            DataNavigator = dataNavigator;

            _ViewCurrentChanging = WeakEventRouter.CreateInstance(this, DataNavigator,
                () => default(CurrentChangingEventArgs),
                (p, d) => p.ViewCurrentChanging += d.Handler,
                (p, d) => p.ViewCurrentChanging -= d.Handler,
                CurrentChanging);

            persistedViewDisposing = (sender, args) =>
                {
                    lock (_Lock)
                    {
                        if (_IsDisposing)
                        {
                            return;
                        }

                        _PersistedView.Disposing -= persistedViewDisposing;
                        _PersistedView = null;

                        if (owner != null)
                        {
                            owner.Dispose();
                        }
                    }
                };

            ownerDisposing = (sender, args) =>
                {
                    lock (_Lock)
                    {
                        if (_IsDisposing)
                        {
                            return;
                        }

                        Dispose();
                        owner.Disposing -= ownerDisposing;
                        if (_PersistedView != null)
                        {
                            _PersistedView.Disposing -= persistedViewDisposing;
                            _PersistedView.Dispose();
                        }
                    }
                };

            _PersistedView.Disposing += persistedViewDisposing;
            owner.Disposing += ownerDisposing;

            regionNavigator.EnqueueUIWork(() =>
                {
                    var viewSource = new CollectionViewSource();

                    Items = new ObservableCollection<T>();

                    _ItemsCollectionChangedRouter = WeakEventRouter.CreateInstance(this, Items,
                        () => default(NotifyCollectionChangedEventArgs),
                        (p, d) => p.CollectionChanged += d.Handler,
                        (p, d) => p.CollectionChanged -= d.Handler,
                        OnItemsCollectionChanged); 

                    viewSource.Source = Items;
                    _PersistedView.View = (ListCollectionView)viewSource.View;

                    Parent = parent;

                    CheckIsRoot();
                });
        }

        public DataNavigatorOwner(INotifyDisposing owner, IDataStoresManager dataStoresManager, IDataNavigator dataNavigator, IPersistedView<T> persistedView, IViewNavigator regionNavigator)
            : this(null, owner, dataStoresManager, dataNavigator, persistedView, regionNavigator) { }

        public DataNavigatorOwner(IDataNavigatorOwner parent, IDataStoresManager dataStoresManager, IDataNavigator dataNavigator, IPersistedView<T> persistedView, IViewNavigator regionNavigator)
            : this(parent, parent, dataStoresManager, dataNavigator, persistedView, regionNavigator) { }

        private void AddChild(IDataNavigatorOwner child)
        {
            EventHandler childDisposing = null;
            IWeakEventRouter _ChildNavigatorRouter = null;

            if (_Children.Contains(child))
            {
                return;
            }

            _Children.Add(child);

            childDisposing = (sender, args) =>
                {
                    child.Disposing -= childDisposing;
                    RemoveChild(child);
                };

            _RemoveActionByChild.Add(child, () =>
                {
                    _ChildNavigatorRouter.CancelSubscription();
                    _Children.Remove(child);
                });

            child.Disposing += childDisposing;

            _ChildNavigatorRouter = WeakEventRouter.CreateInstance(this, child.DataNavigator,
                () => default(PropertyChangedEventArgs),
                (s, d) => s.PropertyChanged += d.Handler,
                (s, d) => s.PropertyChanged -= d.Handler,
                (s, p, a) => ChildDataNavigatorPropertyChanged(p, a));
        }

        private void RemoveChild(IDataNavigatorOwner child)
        {
            if (!_RemoveActionByChild.ContainsKey(child))
            {
                return;
            }
            _RemoveActionByChild[child]();
            _RemoveActionByChild.Remove(child);
        }

        public void PersistChanges(IPersistedView parent)
        {
            PersistingChanges(this, new PersistingChangesEventArgs());
            _PersistedView.PersistChanges(parent);
        }

        public void Dispose()
        {
            if (_IsDisposing)
            {
                return;
            }

            _IsDisposing = true;

            DisposeImpl();
        }

        protected virtual void DisposeImpl()
        {
            Parent = null;
            Disposing(this, EventArgs.Empty);
        }

        private void CheckIsRoot()
        {
            Func<IDataNavigatorOwner, bool> rootCheck = null;
            Func<IDataNavigatorOwner, IDataNavigatorOwner> getFirstGeneration = null;
            Func<IDataNavigatorOwner, bool> isNotDefault = null;

            getFirstGeneration = (child) =>
                {
                    if (child.Parent == null)
                    {
                        return null;
                    }

                    var parentResult = getFirstGeneration(child.Parent);

                    if (parentResult == null)
                    {
                        return this;
                    }

                    return null;
                };

            isNotDefault = (owner) =>
                {
                    if (owner == null)
                    {
                        return true;
                    }

                    if (owner.DataNavigator.HasView)
                    {
                        return true;
                    }

                    return owner.Hierarchy.Any(c => isNotDefault(c));
                };

            rootCheck = (owner) =>
                {
                    if (owner == null)
                    {
                        return false;
                    }

                    return owner.DataNavigator.HasView && !rootCheck(owner.Parent);
                };

            if (!isNotDefault(getFirstGeneration(this)) || rootCheck(this))
            {
                _DataNavigatorPropertyChangedRouter = WeakEventRouter.CreateInstance(this,
                    DataNavigator,
                    () => default(PropertyChangedEventArgs),
                    (p, d) => p.PropertyChanged += d.Handler,
                    (p, d) => p.PropertyChanged -= d.Handler,
                    RootVisibleDataNavigatorPropertyChanged);
            }
        }

        private void ParentDataNavigatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_IgnoreEditStateEvents)
            {
                return;
            }

            _IgnoreEditStateEvents = true;

            var dataNavigator = (IDataNavigator)sender;

            if (e.PropertyName == BindingHelper.Name((IDataNavigator o) => o.EditState))
            {
                switch (dataNavigator.EditState)
                {
                    case EditState.Cancel:
                        {
                            Reload();
                            break;
                        }
                    case EditState.Save:
                        {
                            foreach (var item in Items
                                .Where(i => i.IsEditing))
                            {
                                item.EndEdit();
                            }
                            break;
                        }
                }
            }

            _IgnoreEditStateEvents = false;
        }

        private void ParentDataNavigatorCurrentChanged(object sender, EventArgs e)
        {
            if (_IgnoreEditStateEvents)
            {
                return;
            }

            _IgnoreEditStateEvents = true;

            AllowEdit = Parent.PersistedView.View.CurrentItem != null;

            Reload();

            _IgnoreEditStateEvents = false;
        }

        private void ParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BindingHelper.Name(() => AllowEdit))
            {
                UpdateAllowEdit();
            }
        }

        private void UpdateAllowEdit()
        {
            AllowEdit = (Parent.AllowEdit && Parent.PersistedView.View.CurrentItem != null); 
        }

        private void Reload()
        {
            _PersistedView.RunOpened(() =>
                {
                    Items.Clear();

                    foreach (var item in _PersistedView.ItemsLoader())
                    {
                        Items.Add(item);
                    }
                });
        }

        private void CurrentChanging(DataNavigatorOwner<T> owner, IDataNavigator sender, CurrentChangingEventArgs e)
        {
            EventHandler viewCurrentChanged = null;

            _IgnoreEditStateEvents = true;

            viewCurrentChanged += (o, args) =>
                {
                    sender.ViewCurrentChanged -= viewCurrentChanged;
                    _IgnoreEditStateEvents = false;
                };

            sender.ViewCurrentChanged += viewCurrentChanged;
        }

        private void OnItemsCollectionChanged(DataNavigatorOwner<T> owner, INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs args)
        {
            var handler = ItemsCollectionChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void ChildDataNavigatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_IgnoreEditStateEvents)
            {
                return;
            }

            _IgnoreEditStateEvents = true;

            if (e.PropertyName == BindingHelper.Name((IDataNavigator o) => o.Count)
                || !(sender is IDataNavigator)) 
            {
                DataNavigator.EnterState(EditState.Edit);
            }
            else if (e.PropertyName == BindingHelper.Name((IDataNavigator o) => o.EditState)
                && sender is IDataNavigator
                && ((IDataNavigator)sender).EditState == EditState.Edit)
            {
                DataNavigator.EnterState(EditState.Edit);
            }

            _IgnoreEditStateEvents = false;
        }

        private void RootVisibleDataNavigatorPropertyChanged(DataNavigatorOwner<T> owner, INotifyPropertyChanged sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == BindingHelper.Name((IDataNavigator o) => o.EditState))
            {
                switch (DataNavigator.EditState)
                {
                    case EditState.Save:
                    case EditState.Delete:
                        {
                            _PendingPersistAction = () =>
                                {
                                    IPersistedView persistedViewOwner = Parent == null ? null : Parent.PersistedView;

                                    PersistChanges(persistedViewOwner);
                                    Reload();
                                };
                            break;
                        }
                    default:
                        {
                            if(_PendingPersistAction != null)
                            {
                                _PendingPersistAction();
                            }
                            _PendingPersistAction = null;
                            break;
                        }
                }
            }
        }
    }
}



