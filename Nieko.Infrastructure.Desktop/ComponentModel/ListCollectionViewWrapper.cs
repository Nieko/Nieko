using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace Nieko.Infrastructure.ComponentModel
{
    public abstract class ListCollectionViewWrapper : ICollectionViewWrapper
    {
        protected virtual ListCollectionView View { get; set; }

        public virtual object AddNewItem(object newItem)
        {
            return View.AddNewItem(newItem);
        }

        public virtual bool CanAddNewItem
        {
            get { return View.CanAddNewItem;  }
        }

        public virtual object AddNew()
        {
            return View.AddNew();
        }

        public virtual bool CanAddNew
        {
            get { return View.CanAddNew; }
        }

        public virtual bool CanCancelEdit
        {
            get { return View.CanCancelEdit; }
        }

        public virtual bool CanRemove
        {
            get { return View.CanRemove; }
        }

        public virtual void CancelEdit()
        {
            View.CancelEdit();
        }

        public virtual void CancelNew()
        {
            View.CancelNew();
        }

        public virtual void CommitEdit()
        {
            View.CommitEdit();
        }

        public virtual void CommitNew()
        {
            View.CommitNew();
        }

        public virtual object CurrentAddItem
        {
            get { return View.CurrentAddItem; }
        }

        public virtual object CurrentEditItem
        {
            get { return View.CurrentEditItem; }
        }

        public virtual void EditItem(object item)
        {
            View.EditItem(item);
        }

        public virtual bool IsAddingNew
        {
            get { return View.IsAddingNew; }
        }

        public virtual bool IsEditingItem
        {
            get { return View.IsEditingItem; }
        }

        public virtual NewItemPlaceholderPosition NewItemPlaceholderPosition
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

        public virtual void Remove(object item)
        {
            View.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            View.RemoveAt(index); 
        }

        public virtual bool CanFilter
        {
            get { return View.CanFilter; }
        }

        public virtual bool CanGroup
        {
            get { return View.CanGroup; }
        }

        public virtual bool CanSort
        {
            get { return View.CanSort; }
        }

        public virtual bool Contains(object item)
        {
            return View.Contains(item);
        }

        public virtual System.Globalization.CultureInfo Culture
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

        public virtual event EventHandler CurrentChanged
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

        public virtual event CurrentChangingEventHandler CurrentChanging
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

        public virtual object CurrentItem
        {
            get { return View.CurrentItem; }
        }

        public virtual int CurrentPosition
        {
            get { return View.CurrentPosition; }
        }

        public virtual IDisposable DeferRefresh()
        {
            return View.DeferRefresh();
        }

        public virtual Predicate<object> Filter
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

        public virtual System.Collections.ObjectModel.ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return View.GroupDescriptions; }
        }

        public virtual System.Collections.ObjectModel.ReadOnlyObservableCollection<object> Groups
        {
            get { return View.Groups; }
        }

        public virtual bool IsCurrentAfterLast
        {
            get { return View.IsCurrentAfterLast; }
        }

        public virtual bool IsCurrentBeforeFirst
        {
            get { return View.IsCurrentBeforeFirst; }
        }

        public virtual bool IsEmpty
        {
            get { return View.IsEmpty; }
        }

        public virtual bool MoveCurrentTo(object item)
        {
            return View.MoveCurrentTo(item); 
        }

        public virtual bool MoveCurrentToFirst()
        {
            return View.MoveCurrentToFirst();
        }

        public virtual bool MoveCurrentToLast()
        {
            return View.MoveCurrentToLast();
        }

        public virtual bool MoveCurrentToNext()
        {
            return View.MoveCurrentToNext();
        }

        public virtual bool MoveCurrentToPosition(int position)
        {
            return View.MoveCurrentToPosition(position); 
        }

        public virtual bool MoveCurrentToPrevious()
        {
            return View.MoveCurrentToPrevious(); 
        }

        public virtual void Refresh()
        {
            View.Refresh();
        }

        public virtual SortDescriptionCollection SortDescriptions
        {
            get { return View.SortDescriptions; }
        }

        public virtual System.Collections.IEnumerable SourceCollection
        {
            get { return View.SourceCollection; }
        }

        public virtual System.Collections.IEnumerator GetEnumerator()
        {
            return (View as System.Collections.IEnumerable).GetEnumerator();
        }

        public virtual event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged
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

        public virtual System.Collections.ObjectModel.ReadOnlyCollection<ItemPropertyInfo> ItemProperties
        {
            get { return View.ItemProperties; }
        }

        public virtual int Count
        {
            get { return View.Count; }
        }
    }
}
