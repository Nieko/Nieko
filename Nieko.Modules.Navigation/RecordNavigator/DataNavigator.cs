using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure;
using System.ComponentModel;
using Nieko.Infrastructure.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Data;
using System.Collections.Specialized;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.Windows;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class DataNavigator : IDataNavigator
    {
        private object _Lock = new object();
        private bool _IsDisposing;
        private bool _ProcessChangeEvents = true;
        private bool _IsChangingState = false;
        private static Dictionary<Type, HashSet<string>> _NonEditProperties = new Dictionary<Type, HashSet<string>>();
        
        protected NotifyingFields Fields { get; private set;}

        protected ListCollectionView CurrentView { get; private set;}

        protected IViewNavigator RegionNavigator { get; set; }

        public event EventHandler Disposing;
        public event EventHandler ViewCurrentChanged;
        public event EventHandler<CurrentChangingEventArgs> ViewCurrentChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected T Get<T>(Expression<Func<T>> propertyExpression)
        {
            return Fields.Get(propertyExpression);
        }

        protected void Set<T>(Expression<Func<T>> propertyExpression, T value)
        {
            Fields.Set(propertyExpression, value);
        }

        protected void Set<T>(Expression<Func<T>> propertyExpression, T value, Action valueChanged)
        {
            Fields.Set(propertyExpression, value, valueChanged);
        }

        protected void Set<T>(Expression<Func<T>> propertyExpression, T value, bool forcePropertyChanged)
        {
            Action raiseProperty = null;
            Action clearRaiseProperty = () => raiseProperty = null;

            if (forcePropertyChanged)
            {
                raiseProperty = () =>
                    {
                        var handler = PropertyChanged;

                        if (handler != null)
                        {
                            handler(this, new PropertyChangedEventArgs(BindingHelper.Name(propertyExpression)));
                        }
                    };
            }

            Set(propertyExpression, value, clearRaiseProperty);

            if (raiseProperty != null)
            {
                raiseProperty();
            }
        }

        protected IFormsManager Forms { get; set; }

        public virtual bool HasView
        {
            get
            {
                return false;
            }
        }

        public int Count
        {
            get
            {
                return Get(() => Count);
            }
            set
            {
                Set(() => Count, value);
            }
        }

        public int CurrentPosition
        {
            get
            {
                return Get(() => CurrentPosition);
            }
            set
            {
                Set(() => CurrentPosition,
                    value,
                    () => PerformNavigation(CurrentPosition, RecordNavigation.Position));
            }
        }

        public Action<object> Creator { get; set; }

        public IDataNavigatorOwner Owner
        {
            get
            {
                return Get(() => Owner);
            }
            set
            {
                if (_IsDisposing)
                {
                    return;
                }

                if (Owner == value)
                {
                    return;
                }
                Set(() => Owner, value, ViewChanged);
            }
        }

        public EditState EditState
        {
            get
            {
                return Get(() => EditState);
            }
            private set
            {
                Set(() => EditState, value);
            }
        }

        public DataNavigator(IViewNavigator regionNavigator, IFormsManager formsManager)
        {
            Fields = new NotifyingFields(this, () => PropertyChanged);
            Creator = o => { };

            Forms = formsManager;
            RegionNavigator = regionNavigator;
        }

        public void Navigate(RecordNavigation navigation)
        {
            PerformNavigation(0, navigation);
        }

        public void NavigateTo(int position)
        {
            PerformNavigation(position, RecordNavigation.Position);
        }

        public void Dispose()
        {
            if (_IsDisposing)
            {
                return;
            }

            _IsDisposing = true;
            Set(() => Owner, null, ViewChanged);

            var handler = Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            DisposeImpl();
        }

        public virtual void EnterState(EditState editState)
        {
#if !DEBUG
            try
            {
#endif
                object currentItem;

                if (_IsChangingState)
                {
                    return;
                }

                _IsChangingState = true;

                switch (editState)
                {
                    case EditState.Cancel:
                        if (CurrentView.IsAddingNew)
                        {
                            CurrentView.CancelNew();
                        }
                        else if (CurrentView.IsEditingItem)
                        {
                            CurrentView.CancelEdit();
                        }
                        EditState = editState;
                        EditState = EditState.View;

                        break;
                    case EditState.Delete:
                        currentItem = CurrentView.CurrentItem;
                        CurrentView.Remove(currentItem);
                        EditState = editState;
                        EditState = EditState.View;

                        break;
                    case EditState.Edit:
                        CurrentView.EditItem(CurrentView.CurrentItem);
                        EditState = EditState.Edit;
                        break;
                    case EditState.New:
                        var newInstance = CurrentView.AddNew();
                        Creator(newInstance);

                        EditState = EditState.New;
                        break;
                    case EditState.Save:
                        currentItem = CurrentView.CurrentItem;
                        if (CurrentView.IsAddingNew)
                        {
                            CurrentView.CommitNew();
                        }
                        else if (CurrentView.IsEditingItem)
                        {
                            CurrentView.CommitEdit();
                        }

                        EditState = editState;
                        EditState = EditState.View;
                        break;
                }
#if !DEBUG
            }
            catch (Exception e)
            {
                Forms.HandleFormException("Attempt to " + editState + " failed", e); 
            }
#endif
                _IsChangingState = false;
        }

        public bool Find<T>(Func<T, bool> filter)
            where T : class
        {
            if (CurrentView == null)
            {
                return false;
            }

            var result = CurrentView.Cast<object>()
                .FirstOrDefault(o => o is T && filter((o as T)));

            if (result == null)
            {
                return false;
            }

            CurrentView.MoveCurrentTo(result);

            return true;
        }

        protected virtual void ViewChangedImpl()
        {
            if (CurrentView != null)
            {
                CurrentView.CurrentChanged -= OnViewCurrentChanged;
                CurrentView.CurrentChanging -= OnViewCurrentChanging;
                if (CurrentView.SourceCollection is INotifyCollectionChanged)
                {
                    (CurrentView.SourceCollection as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
                }
            }

            CurrentView = (Owner == null || Owner.PersistedView == null) ?
                null :
                Owner.PersistedView.View;

            if (CurrentView != null)
            {
                CurrentView.CurrentChanged += OnViewCurrentChanged;
                CurrentView.CurrentChanging += OnViewCurrentChanging;
                
                if (CurrentView.SourceCollection is INotifyCollectionChanged)
                {
                    (CurrentView.SourceCollection as INotifyCollectionChanged).CollectionChanged += OnCollectionChanged;
                }
                OnCollectionChanged(CurrentView.SourceCollection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                if (CurrentView.CurrentPosition > -1)
                {
                    OnViewCurrentChanged(CurrentView, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move &&
                e.Action != NotifyCollectionChangedAction.Replace)
            {
                Count = CurrentView.Count;
            }
        }

        protected virtual void DisposeImpl() 
        {
            if (CurrentView != null)
            {
                CurrentView.CurrentChanged -= OnViewCurrentChanged;
                CurrentView.CurrentChanging -= OnViewCurrentChanging;

                if (CurrentView.SourceCollection is INotifyCollectionChanged)
                {
                    (CurrentView.SourceCollection as INotifyCollectionChanged).CollectionChanged -= OnCollectionChanged;
                }
            }
        }

        protected void PerformNavigation(int position, RecordNavigation navigation)
        {
#if !DEBUG
            try
            {
#endif
            switch (navigation)
            {
                case RecordNavigation.First:
                    CurrentView.MoveCurrentToFirst();
                    break;
                case RecordNavigation.Previous:
                    CurrentView.MoveCurrentToPrevious();
                    break;
                case RecordNavigation.Next:
                    CurrentView.MoveCurrentToNext();
                    break;
                case RecordNavigation.Last:
                    CurrentView.MoveCurrentToLast();
                    break;
                case RecordNavigation.Position:
                    NavigateToPosition(position);
                    break;
                case RecordNavigation.Current:
                    OnViewCurrentChanged(CurrentView, EventArgs.Empty);
                    break;
            }
#if !DEBUG
            }
            catch (Exception e)
            {
                Forms.HandleFormException("Navigation to " + navigation.ToString() + " failed", e);  
            }
#endif
        }

        protected virtual void CurrentItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsNonEdit(sender.GetType(), e.PropertyName))
            {
                return;
            }

            if (!_ProcessChangeEvents)
            {
                return;
            }

            if (!(CurrentView.IsEditingItem || CurrentView.IsAddingNew))
            {
                var editableItem = (CurrentView.CurrentItem as IEditableMirrorObject);
                if (editableItem == null || !editableItem.IsEditing)
                {
                    EnterState(EditState.Edit);
                }
            }
        }

        protected bool IsNonEdit(Type type, string propertyName)
        {
            HashSet<string> nonEditProperties = null;

            lock (_Lock)
            {
                if (!_NonEditProperties.TryGetValue(type, out nonEditProperties))
                {
                    nonEditProperties = new HashSet<string>(type.GetPropertiesWithAttribute<NonEditAttribute>()
                        .Select(p => p.Name));

                    _NonEditProperties.Add(type, nonEditProperties);
                }
            }

            return nonEditProperties.Contains(propertyName);
        }

        protected virtual void OnViewCurrentChanged(object sender, EventArgs e)
        {
            if (!_ProcessChangeEvents)
            {
                return;
            }

            _ProcessChangeEvents = false;

            EventHandler handler;

            if (CurrentView.CurrentItem != null && CurrentView.CurrentItem is INotifyPropertyChanged)
            {
                (CurrentView.CurrentItem as INotifyPropertyChanged).PropertyChanged += CurrentItemPropertyChanged;
            }

            CurrentPosition = CurrentView.CurrentPosition + 1;

            handler = ViewCurrentChanged;
            if (handler != null)
            {
                handler(this, e);
            }

            EditState = EditState.View;

            _ProcessChangeEvents = true;
        }

        private void OnViewCurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (!_ProcessChangeEvents)
            {
                return;
            }

            _ProcessChangeEvents = false;

            EventHandler<CurrentChangingEventArgs> handler = ViewCurrentChanging;

            if (handler != null)
            {
                handler.Invoke(this, e);
            }

            if (!e.Cancel)
            {

                if (CurrentView.CurrentItem != null &&
                    (CurrentView.CurrentItem is INotifyPropertyChanged))
                {
                    (CurrentView.CurrentItem as INotifyPropertyChanged).PropertyChanged -= CurrentItemPropertyChanged;
                }

                if (CurrentView.IsEditingItem || CurrentView.IsAddingNew)
                {
                    EnterState(EditState.Save);
                }
            }

            _ProcessChangeEvents = true;
        }

        private void NavigateToPosition(int position)
        {
            CurrentView.MoveCurrentToPosition(position - 1);
        }

        private void ViewChanged()
        {
            RegionNavigator.EnqueueUIWork(ViewChangedImpl);
        }
    }
}
