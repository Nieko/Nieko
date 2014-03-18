using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace Nieko.Infrastructure.Windows.Data
{
    public abstract class ListCollectionViewWrapper : IEditableCollectionViewAddNewItem, ICollectionView
    {
        public virtual ListCollectionView View { get; set; }

        public object AddNewItem(object newItem)
        {
            return View.AddNewItem(newItem);
        }

        public bool CanAddNewItem
        {
            get { return View.CanAddNewItem;  }
        }

        public object AddNew()
        {
            return View.AddNew();
        }

        public bool CanAddNew
        {
            get { return View.CanAddNew; }
        }

        public bool CanCancelEdit
        {
            get { return View.CanCancelEdit; }
        }

        public bool CanRemove
        {
            get { return View.CanRemove; }
        }

        public void CancelEdit()
        {
            View.CancelEdit();
        }

        public void CancelNew()
        {
            View.CancelNew();
        }

        public void CommitEdit()
        {
            View.CommitEdit();
        }

        public void CommitNew()
        {
            View.CommitNew();
        }

        public object CurrentAddItem
        {
            get { return View.CurrentAddItem; }
        }

        public object CurrentEditItem
        {
            get { return View.CurrentEditItem; }
        }

        public void EditItem(object item)
        {
            View.EditItem(item);
        }

        public bool IsAddingNew
        {
            get { return View.IsAddingNew; }
        }

        public bool IsEditingItem
        {
            get { return View.IsEditingItem; }
        }

        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                return View.NewItemPlaceholderPosition;
            }
            set
            {
                View.NewItemPlaceholderPosition = value;
            }
        }

        public void Remove(object item)
        {
            View.Remove(item);
        }

        public void RemoveAt(int index)
        {
            View.RemoveAt(index); 
        }

        public bool CanFilter
        {
            get { return View.CanFilter; }
        }

        public bool CanGroup
        {
            get { return View.CanGroup; }
        }

        public bool CanSort
        {
            get { return View.CanSort; }
        }

        public bool Contains(object item)
        {
            return View.Contains(item);
        }

        public System.Globalization.CultureInfo Culture
        {
            get
            {
                return View.Culture;
            }
            set
            {
                View.Culture = value;
            }
        }

        public event EventHandler CurrentChanged
        {
            add
            {
                View.CurrentChanged += value;
            }
            remove
            {
                View.CurrentChanged -= value;
            }
        }

        public event CurrentChangingEventHandler CurrentChanging
        {
            add
            {
                View.CurrentChanging += value;
            }
            remove
            {
                View.CurrentChanging -= value;
            }
        }

        public object CurrentItem
        {
            get { return View.CurrentItem; }
        }

        public int CurrentPosition
        {
            get { return View.CurrentPosition; }
        }

        public IDisposable DeferRefresh()
        {
            return View.DeferRefresh();
        }

        public Predicate<object> Filter
        {
            get
            {
                return View.Filter;
            }
            set
            {
                View.Filter = value;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return View.GroupDescriptions; }
        }

        public System.Collections.ObjectModel.ReadOnlyObservableCollection<object> Groups
        {
            get { return View.Groups; }
        }

        public bool IsCurrentAfterLast
        {
            get { return View.IsCurrentAfterLast; }
        }

        public bool IsCurrentBeforeFirst
        {
            get { return View.IsCurrentBeforeFirst; }
        }

        public bool IsEmpty
        {
            get { return View.IsEmpty; }
        }

        public bool MoveCurrentTo(object item)
        {
            return View.MoveCurrentTo(item); 
        }

        public bool MoveCurrentToFirst()
        {
            return View.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return View.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return View.MoveCurrentToNext();
        }

        public bool MoveCurrentToPosition(int position)
        {
            return View.MoveCurrentToPosition(position); 
        }

        public bool MoveCurrentToPrevious()
        {
            return View.MoveCurrentToPrevious(); 
        }

        public void Refresh()
        {
            View.Refresh();
        }

        public SortDescriptionCollection SortDescriptions
        {
            get { return View.SortDescriptions; }
        }

        public System.Collections.IEnumerable SourceCollection
        {
            get { return View.SourceCollection; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return (View as System.Collections.IEnumerable).GetEnumerator();
        }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                (View as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += value;
            }
            remove
            {
                (View as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged -= value;
            }
        }
    }
}
