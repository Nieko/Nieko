using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure;
using System.Collections.ObjectModel;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Linq.Expressions;
using System.ComponentModel;
using Nieko.Infrastructure.Collections;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Base class for representing a one-one relationship in 
    /// a ModelView graph
    /// </summary>
    /// <remarks>
    /// Not necessarily the top-most or edge node in the graph.
    /// Wraps the collection logic for <seealso cref="IPersistedView"/> into
    /// a singleton instance while still implementing the interface.
    /// Handy base class for the graph root
    /// </remarks>
    /// <typeparam name="T">ModelView</typeparam>
    public abstract class SingletonPersistedView<T> : ListCollectionViewWrapper, IPersistedView<T>
        where T : IEditableMirrorObject
    {
        private readonly Func<IDataNavigatorOwnerBuilder> _BuilderFactory;
        private readonly IViewNavigator _RegionNavigator;

        private bool _IsDisposing = false;
        private T _Instance;
        private List<T> _SingletonWrapper;
        private ReadOnlyDictionary<PrimaryKey> _DeletedItems; 
        private ReadOnlyCollection<T> _SingletonCollection;
        private IWeakEventRouter _ViewCurrentChangedRouter;

        public event EventHandler Disposing;
        public event EventHandler Loaded;
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual T Instance 
        {
            get
            {
                if (Owner == null)
                {
                    return _Instance;
                }

                ICollection<T> wrapper = (Owner.PersistedView.View.SourceCollection as ICollection<T>);

                if (wrapper.Count == 0)
                {
                    return _Instance;
                }
                else
                {
                    return wrapper.First();
                }
            }
            set
            {
                if (Owner == null)
                {
                    _Instance = value;
                    return;
                }

                ICollection<T> wrapper = (Owner.PersistedView.View.SourceCollection as ICollection<T>);

                if (value == null)
                {
                    if (wrapper.Count == 0)
                    {
                        return;
                    }

                    wrapper.Clear();
                }
                else
                {
                    if (wrapper.Count > 0)
                    {
                        wrapper.Clear();
                    }
                    wrapper.Add(value);
                }
            }
        }

        T IPersistedView<T>.CurrentItem
        {
            get { return Instance; }
        }

        int IPersistedView.CurrentPosition
        {
            get
            {
                return Instance == null ? -1 : 0;
            }
            set
            {
                throw new IndexOutOfRangeException();  
            }
        }

        public IDataNavigatorOwner Owner { get; private set; }

        public ObservableCollection<T> Items { get; set; }

        public ReadOnlyDictionary<PrimaryKey> DeletedItems
        {
            get
            {
                if (_DeletedItems == null)
                {
                    _DeletedItems = ReadOnlyDictionary<PrimaryKey>.Create(new Dictionary<PrimaryKey, T>()); 
                }

                return _DeletedItems;
            }
        }

        protected IDataStoresManager DataStoresManager { get; private set; }

        protected INotifyModelViewGraphNodeLoaded LoadedPublisher { get; private set; }

        public SingletonPersistedView(Func<IDataNavigatorOwnerBuilder> builderFactory, IDataStoresManager dataStoresManager, IViewNavigator regionNavigator, INotifyModelViewGraphNodeLoaded loadedPublisher)
        {
            _SingletonWrapper = new List<T>();
            _SingletonCollection = new ReadOnlyCollection<T>(_SingletonWrapper); 

            _BuilderFactory = builderFactory;
            DataStoresManager = dataStoresManager;
            _RegionNavigator = regionNavigator;

            LoadedPublisher = loadedPublisher;

            EventHandler ownerLoadedHandler = null;

            ownerLoadedHandler = (sender, args) =>
               {
                   LoadBase();

                   RaiseLoaded();

                   _RegionNavigator.EnqueueUIWork(() =>
                    {
                        (Owner.PersistedView.View.SourceCollection as ICollection<T>).Add(_Instance);
                        Owner.PersistedView.View.MoveCurrentToFirst();
                    });

                   LoadedPublisher.Loaded -= ownerLoadedHandler;
               };

            LoadedPublisher.Loaded += ownerLoadedHandler;
        }

        private void LoadBase()
        {
            var builder = _BuilderFactory();

            Owner = builder.CreateDataNavigator(this)
                .UsingPersistedView<T>(this)
                .Build();

            _ViewCurrentChangedRouter = WeakEventRouter.CreateInstance(this, Owner.DataNavigator,
                () => default(EventArgs),
                (p, d) => p.ViewCurrentChanged += d.Handler,
                (p, d) => p.ViewCurrentChanged -= d.Handler,
                ViewCurrentChanged); 

            LoadImpl();
        }

        public abstract void PersistChanges(IPersistedView owner);

        protected abstract void LoadImpl();

        private void RaiseLoaded()
        {
            var handler = Loaded;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public abstract void RunOpened(Action storeOpenedAction);

        public IEnumerable<T> ItemsLoader()
        {
            return _SingletonCollection;
        }

        public void Dispose()
        {
            if (_IsDisposing)
            {
                return;
            }

            _IsDisposing = true;

            DisposeImpl();

            var handler = Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void DisposeImpl() { }

        protected void ViewCurrentChanged(SingletonPersistedView<T> owner, IDataNavigator sender, EventArgs args)
        {
            RaisePropertyChanged(BindingHelper.Name(()=> CurrentItem)); 
            RaisePropertyChanged(BindingHelper.Name(()=> CurrentPosition));
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName)); 
            }
        }
    }
}
