#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data.SampleData
{
    public class PersistedViewSample<T> : ListCollectionViewWrapper, IPersistedView<T>
        where T : IEditableMirrorObject
    {
        public event EventHandler Disposing = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler Loaded = delegate { };

        private ObservableCollection<T> _Items;
        
        public IDataNavigatorOwner Owner { get; private set; }

        public new int CurrentPosition
        {
            get
            {
                return this.CurrentPosition;
            }
            set
            {
                MoveCurrentToPosition(value);
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
                if (_Items != null)
                {
                    throw new InvalidOperationException("Items Collection is in use and cannot be changed"); 
                }

                if (value == null)
                {
                    return;
                }

                _Items = value;
                View = CollectionViewSource.GetDefaultView(_Items) as ListCollectionView; 
            }
        }

        public ReadOnlyDictionary<PrimaryKey> DeletedItems { get; set; }

        public new T CurrentItem
        {
            get { return (T)base.CurrentItem; }
        }

        public PersistedViewSample()
        {
            Items = new ObservableCollection<T>();
        }

        public void PersistChanges(IPersistedView parent) { }

        public void RunOpened(Action storeOpenedAction) { }

        public void Dispose() { }

        public IEnumerable<T> ItemsLoader()
        {
            return Items;
        }
    }
}

#endif