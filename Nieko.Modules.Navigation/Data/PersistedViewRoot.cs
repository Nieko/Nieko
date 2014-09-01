using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.ComponentModel;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Nieko.Infrastructure.ViewModel;
using System.Windows.Data;
using Nieko.Infrastructure.Windows.Data;
using Nieko.Infrastructure;

namespace Nieko.Modules.Navigation.Data
{
    internal class PersistedViewRoot : SingletonPersistedView<IPersistedViewRoot>, IPersistedViewRoot, IPrimaryKeyed
    {
        private INotifyDisposing _Root;

        public INotifyDisposing Root 
        {
            get
            {
                return _Root;
            }
            set
            {
                if ((object)_Root == (object)value)
                {
                    return;
                }

                if (_Root != null)
                {
                    _Root.Disposing -= RootDisposing;
                }

                _Root = value;

                if (value != null)
                {
                    _Root.Disposing += RootDisposing;
                }
            }
        }

        [PrimaryKey]
        public PersistedViewRoot Key
        {
            get
            {
                return this;
            }
        }

        public IDataStoresManager Manager
        {
            get
            {
                return DataStoresManager;
            }
        }

        public IViewNavigator ViewNavigator { get; private set; }

        public bool IsReadOnly { get { return false; } }

        public bool IsEditing { get; private set; }

        public bool SuppressNotifications { get; set; }

        public bool HasChanged { get; set; }

        public PrimaryKey PrimaryKey
        {
            get
            {
                return SourceKey;
            }
        }

        public PrimaryKey SourceKey { get; set; }

        public PersistedViewRoot(Func<ITierCoordinatorBuilder> builderFactory, IDataStoresManager dataStoresManager, IViewNavigator regionNavigator, LoadProcessStarter processStarter)
            : base(builderFactory, dataStoresManager, regionNavigator, processStarter)
        {
            SourceKey = new PrimaryKey(this);
            ViewNavigator = regionNavigator;

            Instance = this;
        }

        public void Load()
        {
            (LoadedPublisher as LoadProcessStarter).Start();
        }

        void IEditableObject.BeginEdit()
        {
            if (IsEditing)
            {
                throw new InvalidOperationException("Already Editing");
            }

            IsEditing = true;
        }

        void IEditableObject.CancelEdit()
        {
            if (!IsEditing)
            {
                throw new InvalidOperationException("Not Editing");
            }

            IsEditing = false;
        }

        void IEditableObject.EndEdit()
        {
            if (!IsEditing)
            {
                throw new InvalidOperationException("Not Editing");
            }

            HasChanged = true;
            IsEditing = false;
        }

        public override void RunOpened(Action storeOpenedAction)
        {
            storeOpenedAction();
        }

        public void Reload()
        {
            Owner.DataNavigator.Navigate(RecordNavigation.Current); 
        }

        protected override void LoadImpl() {}

        public override void PersistChanges(IPersistedView owner) {}

        public int CompareTo(object obj)
        {
            if(this == obj)
            {
                return 0;
            }

            return this.GetHashCode().CompareTo(obj == null ? 0 : obj.GetHashCode()); 
        }

        private void RootDisposing(object sender, EventArgs args)
        {
            Dispose();
            _Root.Disposing -= RootDisposing;
        }
    }
}
