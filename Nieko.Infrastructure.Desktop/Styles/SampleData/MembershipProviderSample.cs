#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.Windows.Input;
using System.ComponentModel;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Styles.SampleData
{
    public class MembershipProviderSample : ListCollectionViewWrapper, IMembershipProvider<MembershipProviderLineItemSample>
    {
        public event EventHandler ViewChanged = delegate { };

        public ObservableCollection<MembershipProviderLineItemSample> AvailableMemberships { get; private set; }

        public MembershipProviderLineItemSample NewItem { get; set; }

        public Func<IMembershipProvider<MembershipProviderLineItemSample>, bool> RemoveAllowed { get; set; }

        public ObservableCollection<MembershipProviderLineItemSample> Items { get; set; }

        public ReadOnlyDictionary<PrimaryKey> DeletedItems { get; private set; }

        public IEnumerable<MembershipProviderLineItemSample> ItemsLoader() { return null; }

        public ITierCoordinator Owner { get; private set; }

        MembershipProviderLineItemSample IPersistedView<MembershipProviderLineItemSample>.CurrentItem
        {
            get { return (MembershipProviderLineItemSample)CurrentItem; }
        }

        int IPersistedView.CurrentPosition
        {
            get
            {
                return CurrentPosition;
            }
            set
            {
                MoveCurrentToPosition(value);
            }
        }

        public MembershipProviderSample()
        {
            AvailableMemberships = new ObservableCollection<MembershipProviderLineItemSample>()
            {
                new MembershipProviderLineItemSample(),
                new MembershipProviderLineItemSample(),
                new MembershipProviderLineItemSample()
            };
            NewItem = new MembershipProviderLineItemSample();
            RemoveAllowed = mp => true;
            Items = new ObservableCollection<MembershipProviderLineItemSample>()
            {
                new MembershipProviderLineItemSample(),
                new MembershipProviderLineItemSample()
            };
            
            View = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            MoveCurrentToPosition(1); 
        }

        public void PersistChanges(IPersistedView parent) { }

        public void RunOpened(Action storeOpenedAction) { }

        public event EventHandler Disposing = delegate { };

        public void Dispose()
        {
            Disposing(this, EventArgs.Empty);
        }

        public event EventHandler<PersistedViewPersistingEventArgs> Persisting = delegate { };

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public event EventHandler Loaded = delegate { };

        public event EventHandler Reset = delegate { };

        object IMembershipProvider.NewItem
        {
            get
            {
                return this.NewItem;
            }
            set
            {
                this.NewItem = (MembershipProviderLineItemSample)value;
            }
        }

        public object Options
        {
            get { return this.Options; }
        }

        public ICommand AddCommand
        {
            get { return null; }
        }

        public bool AddEnabled
        {
            get { return true; }
        }

        public ICommand RemoveCommand
        {
            get { return null; }
        }

        public void SetSource(System.Collections.IList items) { }
    }
}
#endif