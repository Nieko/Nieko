using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Nieko.Infrastructure;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.Windows;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class DataNavigatorViewModel : DataNavigator, IDataNavigatorViewModel
    {
        private static List<EditState> _EditStateTypes;
        private static List<RecordNavigation> _NavigationTypes;

        private Dictionary<EditState, ICommand> _EditStateCommands;
        private ObservableCollection<EditState> _EditStatesEnabled;
        private ObservableCollection<EditState> _EditStatesVisible;
        private Dictionary<RecordNavigation, ICommand> _NavigationCommands;
        private ObservableCollection<RecordNavigation> _NavigationsEnabled;
        private ObservableCollection<RecordNavigation> _NavigationsVisible;
        private bool _ShowNavigators;
        private INavigatorVisibilityProvider _VisibilityProvider;
        private IWeakEventRouter _VisibilityProviderRouter;

        public static IList<EditState> EditStateTypes
        {
            get
            {
                if (_EditStateTypes == null)
                {
                    _EditStateTypes = new List<EditState>();

                    EditState editState;
                    foreach (string navigationName in Enum.GetNames(typeof(EditState)))
                    {
                        editState = (EditState)Enum.Parse(typeof(EditState), navigationName);
                        _EditStateTypes.Add(editState);
                    }
                }
                return _EditStateTypes;
            }
        }

        public static IList<RecordNavigation> NavigationTypes
        {
            get
            {
                if (_NavigationTypes == null)
                {
                    _NavigationTypes = new List<RecordNavigation>();

                    RecordNavigation navigation;
                    foreach (string navigationName in Enum.GetNames(typeof(RecordNavigation)))
                    {
                        navigation = (RecordNavigation)Enum.Parse(typeof(RecordNavigation), navigationName);
                        _NavigationTypes.Add(navigation);
                    }
                }
                return _NavigationTypes;
            }
        }

        public override bool HasView
        {
            get
            {
                return true;
            }
        }

        public ICommand Cancel
        {
            get
            {
                return EditStateCommands[EditState.Cancel];
            }
        }

        public ICommand Delete
        {
            get
            {
                return EditStateCommands[EditState.Delete];
            }
        }

        public ICommand Edit
        {
            get
            {
                return EditStateCommands[EditState.Edit];
            }
        }

        public ICommand First
        {
            get
            {
                return NavigationCommands[RecordNavigation.First];
            }
        }

        public ICommand GoToPosition
        {
            get
            {
                return NavigationCommands[RecordNavigation.Position];
            }
        }

        public ICommand Last
        {
            get
            {
                return NavigationCommands[RecordNavigation.Last];
            }
        }

        public ICommand New
        {
            get
            {
                return EditStateCommands[EditState.New];
            }
        }

        public ICommand Next
        {
            get
            {
                return NavigationCommands[RecordNavigation.Next];
            }
        }

        public ICommand Previous
        {
            get
            {
                return NavigationCommands[RecordNavigation.Previous];
            }
        }

        public ICommand Save
        {
            get
            {
                return EditStateCommands[EditState.Save];
            }
        }

        public Visibility CancelVisible
        {
            get 
            { 
                return Get(()=>CancelVisible); 
            }
            set
            {
                Set(() => CancelVisible, value); 
            }
        }

        public Visibility CountVisible
        {
            get
            {
                return Get(() => CountVisible);
            }
            set
            {
                Set(() => CountVisible, value);
            }
        }

        public Visibility DeleteVisible
        {
            get
            {
                return Get(() => DeleteVisible);
            }
            set
            {
                Set(() => DeleteVisible, value);
            }
        }

        public Visibility EditVisible
        {
            get
            {
                return Get(() => EditVisible);
            }
            set
            {
                Set(() => EditVisible, value);
            }
        }

        public Visibility FirstVisible
        {
            get
            {
                return Get(() => FirstVisible);
            }
            set
            {
                Set(() => FirstVisible, value);
            }
        }

        public Visibility GoToPositionVisible
        {
            get
            {
                return Get(() => GoToPositionVisible);
            }
            set
            {
                Set(() => GoToPositionVisible, value);
            }
        }

        public Visibility LastVisible
        {
            get
            {
                return Get(() => LastVisible);
            }
            set
            {
                Set(() => LastVisible, value);
            }
        }

        public Visibility NewVisible
        {
            get
            {
                return Get(() => NewVisible);
            }
            set
            {
                Set(() => NewVisible, value);
            }
        }

        public Visibility NextVisible
        {
            get
            {
                return Get(() => NextVisible);
            }
            set
            {
                Set(() => NextVisible, value);
            }
        }

        public Visibility PreviousVisible
        {
            get
            {
                return Get(() => PreviousVisible);
            }
            set
            {
                Set(() => PreviousVisible, value);
            }
        }

        public Visibility SaveVisible
        {
            get
            {
                return Get(() => SaveVisible);
            }
            set
            {
                Set(() => SaveVisible, value);
            }
        }

        public Visibility ToolBarVisibility
        {
            get
            {
                return Get(() => ToolBarVisibility);
            }
            set
            {
                Set(() => ToolBarVisibility, value);
            }
        }

        public Dictionary<EditState, ICommand> EditStateCommands
        {
            get
            {
                if (_EditStateCommands == null)
                {
                    EditState editState;
                    _EditStateCommands = new Dictionary<EditState, ICommand>();

                    foreach (string editStateName in Enum.GetNames(typeof(EditState)))
                    {
                        editState = (EditState)Enum.Parse(typeof(EditState), editStateName);
                        if (editState == EditState.None)
                        {
                            continue;
                        }
                        _EditStateCommands.Add(editState, new EditStateCommand((sender, state) => EnterState(state), editState, CanEnterState));
                    }
                }
                return _EditStateCommands;
            }
        }

        public ObservableCollection<EditState> EditStatesEnabled
        {
            get
            {
                if (_EditStatesEnabled == null)
                {
                    _EditStatesEnabled = new ObservableCollection<EditState>();
                }
                return _EditStatesEnabled;
            }
        }

        public ObservableCollection<EditState> EditStatesVisible
        {
            get
            {
                if (_EditStatesVisible == null)
                {
                    _EditStatesVisible = new ObservableCollection<EditState>();
                    UpdateVisibleEditStates();
                }
                return _EditStatesVisible;
            }
        }

        public Dictionary<RecordNavigation, ICommand> NavigationCommands
        {
            get
            {
                if (_NavigationCommands == null)
                {
                    RecordNavigation navigation;
                    _NavigationCommands = new Dictionary<RecordNavigation, ICommand>();

                    foreach (string navigationName in Enum.GetNames(typeof(RecordNavigation)))
                    {
                        navigation = (RecordNavigation)Enum.Parse(typeof(RecordNavigation), navigationName);
                        if (navigation == RecordNavigation.None)
                        {
                            continue;
                        }
                        _NavigationCommands.Add(navigation, new RecordNavigationCommand(PerformNavigation, navigation, CanNavigate));
                    }
                }
                return _NavigationCommands;
            }
        }

        public ObservableCollection<RecordNavigation> NavigationsEnabled
        {
            get
            {
                if (_NavigationsEnabled == null)
                {
                    _NavigationsEnabled = new ObservableCollection<RecordNavigation>();
                }
                return _NavigationsEnabled;
            }
        }

        public ObservableCollection<RecordNavigation> NavigationsVisible
        {
            get
            {
                if (_NavigationsVisible == null)
                {
                    _NavigationsVisible = new ObservableCollection<RecordNavigation>();
                    UpdateVisibleNavigationStates();
                }
                return _NavigationsVisible;
            }
        }
        
        public bool ShowNavigators
        {
            get
            {
                return _ShowNavigators;
            }
            set
            {
                if (_ShowNavigators == value)
                {
                    return;
                }
                _ShowNavigators = value;
                UpdateVisibleNavigationStates();
            }
        }

        public INavigatorVisibilityProvider VisibilityProvider
        {
            get
            {
                return _VisibilityProvider;
            }
            set
            {
                if (_VisibilityProvider == value)
                {
                    return;
                }
                if (_VisibilityProvider != null)
                {
                    _VisibilityProviderRouter.CancelSubscription();
                }
                _VisibilityProvider = value;
                if (_VisibilityProvider != null)
                {
                    _VisibilityProviderRouter = WeakEventRouter.CreateInstance(this,
                        _VisibilityProvider,
                        () => default(PropertyChangedEventArgs),
                        (p, d) => p.PropertyChanged += d.Handler,
                        (p, d) => p.PropertyChanged -= d.Handler,
                        (s, p, a) => OnVisibilityProviderPropertyChanged(p, a));
                }
                UpdateVisibility();
                UpdateVisibleEditStates();
                UpdateVisibleNavigationStates();
            }
        }

        public DataNavigatorViewModel(IViewNavigator regionNavigator, IFormsManager forms)
            : base(regionNavigator, forms)
        {
            VisibilityProvider = new SimpleVisibilityProvider();

            ShowNavigators = true;
            NavigationsVisible.CollectionChanged += NavigationsVisible_CollectionChanged;
            EditStatesVisible.CollectionChanged += EditStatesVisible_CollectionChanged;
        }

        protected override void DisposeImpl()
        {
            base.DisposeImpl();
            
            if (NavigationsVisible != null)
            {
                NavigationsVisible.CollectionChanged -= NavigationsVisible_CollectionChanged;
            }

            if (EditStatesVisible != null)
            {
                EditStatesVisible.CollectionChanged -= EditStatesVisible_CollectionChanged;
            }

            _VisibilityProvider = null;
        }

        protected override void ViewChangedImpl()
        {
            base.ViewChangedImpl();

            if (CurrentView != null)
            {
                UpdateEnablers();
            }
        }

        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(sender, e);

            if (e.Action != NotifyCollectionChangedAction.Move &&
                e.Action != NotifyCollectionChangedAction.Replace)
            {
                UpdateEnablers();
            }
        }

        protected override void OnViewCurrentChanged(object sender, EventArgs e)
        {
            base.OnViewCurrentChanged(sender, e);

            UpdateEnablers();
        }

        protected override void CurrentItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsNonEdit(sender.GetType(), e.PropertyName))
            {
                return;
            }

            base.CurrentItemPropertyChanged(sender, e);

            UpdateEnablers();
        }

        public override void EnterState(EditState editState)
        {
            base.EnterState(editState);

            UpdateEnablers();
        }

        public bool IsEditStateVisible(Control editStateControl)
        {
            foreach (CommandBinding binding in editStateControl.CommandBindings)
            {
                if (!(binding.Command is EditStateCommand))
                {
                    continue;
                }

                if (!EditStatesVisible.Contains((binding.Command as EditStateCommand).EditState))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsNavigationVisible(Control navigationControl)
        {
            foreach (CommandBinding binding in navigationControl.CommandBindings)
            {
                if (!(binding.Command is RecordNavigationCommand))
                {
                    continue;
                }

                if (!NavigationsVisible.Contains((binding.Command as RecordNavigationCommand).Navigation))
                {
                    return false;
                }
            }

            return true;
        }

        private void PerformNavigation(object parameter, RecordNavigation navigation)
        {
            if (navigation == RecordNavigation.Position)
            {
                int position;

                if (!((parameter is TextBox) && int.TryParse((parameter as TextBox).Text, out position)))
                {
                    Set(() => CurrentPosition, CurrentPosition, true);
                    return;
                }

                if (!(parameter is TextBox) && !int.TryParse(parameter.ToString(), out position))
                {
                    Set(() => CurrentPosition, CurrentPosition, true);
                    return;
                }

                base.PerformNavigation(position, navigation);
            }
            else
            {
                base.PerformNavigation(0, navigation);
            }
        }

        private bool CanEnterState(object parameter, EditState editState)
        {
            return EditStatesEnabled.Contains(editState);
        }

        private bool CanNavigate(object parameter, RecordNavigation navigation)
        {
            if (navigation == RecordNavigation.Position)
            {
                int dummy;
                return NavigationsEnabled.Contains(navigation) &&
                        parameter is TextBox ||
                        int.TryParse((parameter as TextBox).Text, out dummy);
            }
            return NavigationsEnabled.Contains(navigation);
        }

        private void EditStatesVisible_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFirstVisible();
            UpdatePreviousVisible();
            UpdateNextVisible();
            UpdateLastVisible();
            UpdateGoToPositionVisible();
            UpdateCountVisible();
        }

        private void NavigationsVisible_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateNewVisible();
            UpdateEditVisible();
            UpdateSaveVisible();
            UpdateCancelVisible();
            UpdateDeleteVisible();
        }

        private void OnVisibilityProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BindingHelper.Name((INavigatorVisibilityProvider v) => v.ShowNavigationBar))
            {
                UpdateVisibility();
            }

            if (e.PropertyName == BindingHelper.Name((INavigatorVisibilityProvider v) => v.ShowAllEditStates)
                || e.PropertyName == BindingHelper.Name((INavigatorVisibilityProvider v) => v.VisibleEditStates))
            {
                UpdateVisibleEditStates();
            }

            if (e.PropertyName == BindingHelper.Name((INavigatorVisibilityProvider v) => v.ShowAllNavigationStates)
                || e.PropertyName == BindingHelper.Name((INavigatorVisibilityProvider v) => v.VisibleNavigationStates))
            {
                UpdateVisibleNavigationStates();
            }
        }

        private void UpdateCancelVisible()
        {
            CancelVisible = EditStatesVisible.Contains(EditState.Cancel) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateCountVisible()
        {
            CountVisible = NavigationsVisible.Contains(RecordNavigation.Count) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateCurrentPosition()
        {
            CurrentPosition = CurrentView.CurrentPosition + 1;
        }

        private void UpdateDeleteVisible()
        {
            DeleteVisible = EditStatesVisible.Contains(EditState.Delete) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateEditVisible()
        {
            EditVisible = EditStatesVisible.Contains(EditState.Edit) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateEnablers()
        {
            NavigationsEnabled.Clear();
            EditStatesEnabled.Clear();

            if (CurrentView == null || (!Owner.AllowEdit)) 
            {
                CurrentPosition = 0;
                return;
            }

            CurrentPosition = CurrentView.CurrentPosition + 1;
            Count = CurrentView.Count;

            NavigationsEnabled.Add(RecordNavigation.Count);
            if (Count > 0)
            {
                NavigationsEnabled.Add(RecordNavigation.Position);
                if (CurrentPosition > 1)
                {
                    NavigationsEnabled.Add(RecordNavigation.First);
                    NavigationsEnabled.Add(RecordNavigation.Previous);
                }

                if (CurrentPosition < Count)
                {
                    NavigationsEnabled.Add(RecordNavigation.Next);
                    NavigationsEnabled.Add(RecordNavigation.Last);
                }

            }
            if (!(CurrentView.IsAddingNew || CurrentView.IsEditingItem))
            {
                EditStatesEnabled.Add(EditState.New);
            }
            if (CurrentPosition > 0)
            {
                
                EditStatesEnabled.Add(EditState.View);
                EditStatesEnabled.Add(EditState.Edit);
                if (!(CurrentView.IsAddingNew || CurrentView.IsEditingItem))
                {
                    EditStatesEnabled.Add(EditState.Delete);
                }
            }

            if (CurrentView.IsAddingNew || CurrentView.IsEditingItem)
            {
                EditStatesEnabled.Add(EditState.Save);
                EditStatesEnabled.Add(EditState.Cancel);
            }
        }

        private void UpdateFirstVisible()
        {
            FirstVisible = NavigationsVisible.Contains(RecordNavigation.First) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateGoToPositionVisible()
        {
            GoToPositionVisible = NavigationsVisible.Contains(RecordNavigation.Position) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateLastVisible()
        {
            LastVisible = NavigationsVisible.Contains(RecordNavigation.Last) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateNewVisible()
        {
            NewVisible = EditStatesVisible.Contains(EditState.New) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateNextVisible()
        {
            NextVisible = NavigationsVisible.Contains(RecordNavigation.Next) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdatePreviousVisible()
        {
            PreviousVisible = NavigationsVisible.Contains(RecordNavigation.Previous) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateSaveVisible()
        {
            SaveVisible = EditStatesVisible.Contains(EditState.Save) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateVisibility()
        {
            ToolBarVisibility = VisibilityProvider.ShowNavigationBar ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void UpdateVisibleEditStates()
        {
            EditStatesVisible.Clear();

            if (VisibilityProvider == null || VisibilityProvider.ShowAllEditStates)
            {
                foreach (var state in EditStateTypes)
                {
                    EditStatesVisible.Add(state);
                }

                return;
            }

            foreach (var state in VisibilityProvider.VisibleEditStates)
            {
                EditStatesVisible.Add(state);
            }
        }

        private void UpdateVisibleNavigationStates()
        {
            NavigationsVisible.Clear();
            if (!ShowNavigators)
            {
                return;
            }

            if (VisibilityProvider == null || VisibilityProvider.ShowAllNavigationStates)
            {
                foreach (var navigationType in NavigationTypes)
                {
                    if (navigationType == RecordNavigation.None)
                    {
                        continue;
                    }
                    NavigationsVisible.Add(navigationType);
                }

                return;
            }

            foreach (var navigationType in VisibilityProvider.VisibleNavigationStates)
            {
                NavigationsVisible.Add(navigationType);
            }
        }
    }
}