#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class DataNavigatorViewModelSample : IDataNavigatorViewModel
    {
        private INavigatorVisibilityProvider _VisibilityProvider;

        public ICommand Cancel
        {
            get { return null; }
        }

        public ICommand Delete
        {
            get { return null; }
        }

        public ICommand Edit
        {
            get { return null; }
        }

        public ICommand First
        {
            get { return null; }
        }

        public ICommand GoToPosition
        {
            get { return null; }
        }

        public ICommand Last
        {
            get { return null; }
        }

        public ICommand New
        {
            get { return null; }
        }

        public ICommand Next
        {
            get { return null; }
        }

        public ICommand Previous
        {
            get { return null; }
        }

        public ICommand Save
        {
            get { return null; }
        }

        public Visibility CancelVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility CountVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility DeleteVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility EditVisible
        {
            get
            {
                return Visibility.Collapsed;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility FirstVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility GoToPositionVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility LastVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility NewVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility NextVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility PreviousVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility SaveVisible
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Visibility ToolBarVisibility
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<EditState, ICommand> EditStateCommands
        {
            get { return null; }
        }

        public ObservableCollection<EditState> EditStatesEnabled
        {
            get { return null; }
        }

        public ObservableCollection<EditState> EditStatesVisible
        {
            get { return null; }
        }

        public Dictionary<RecordNavigation, ICommand> NavigationCommands
        {
            get { return null; }
        }

        public ObservableCollection<RecordNavigation> NavigationsEnabled
        {
            get { return null; }
        }

        public ObservableCollection<RecordNavigation> NavigationsVisible
        {
            get { return null; }
        }

        public bool ShowNavigators
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public INavigatorVisibilityProvider VisibilityProvider
        {
            get
            {
                if (_VisibilityProvider == null)
                {
                    _VisibilityProvider = new SimpleVisibilityProvider();
                }

                return _VisibilityProvider;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler ViewCurrentChanged = delegate { };

        public event EventHandler<CurrentChangingEventArgs> ViewCurrentChanging = delegate { };

        public bool HasView
        {
            get { return true; }
        }

        public int Count
        {
            get
            {
                return 23;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int CurrentPosition
        {
            get
            {
                return 1;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Action<object> Creator
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IDataNavigatorOwner Owner
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public EditState EditState
        {
            get { return EditState.View; }
        }

        public void Navigate(RecordNavigation navigation)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(int position)
        {
            throw new NotImplementedException();
        }

        public void EnterState(EditState state)
        {
            throw new NotImplementedException();
        }

        public bool Find<T>(Func<T, bool> filter) where T : class
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public event EventHandler Disposing = delegate { };

        public void Dispose()
        {
            Disposing(this, EventArgs.Empty); 
        }
    }
}
#endif