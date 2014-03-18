using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nieko.Infrastructure;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Standard implementation of Data Navigation control visibility.
    /// </summary>
    /// <remarks>
    /// User interface control for Edit is hidden (entering Edit <seealso cref="EditState"/>)
    /// by default; all other Editing and Navigation actions are visible
    /// </remarks>
    public class SimpleVisibilityProvider : INavigatorVisibilityProvider
    {
        private static List<EditState> _AllEditStates = new List<EditState>(Enum.GetValues(typeof(EditState)).Cast<EditState>());
        private static List<RecordNavigation> _AllNavigationStates = new List<RecordNavigation>(Enum.GetValues(typeof(RecordNavigation)).Cast<RecordNavigation>());

        private bool _ShowAllEditStates = false;
        private bool _ShowAllNavigationStates = true;
        private bool _ShowNavigationBar = true;
        private ObservableCollection<EditState> _VisibleEditStates;
        private ObservableCollection<RecordNavigation> _VisibleNavigationStates;

        public SimpleVisibilityProvider()
        {
            _VisibleEditStates = new ObservableCollection<EditState>(_AllEditStates
                .Where(es => es != EditState.Edit));
            _VisibleNavigationStates = new ObservableCollection<RecordNavigation>(_AllNavigationStates);

            _VisibleEditStates.CollectionChanged += VisibleEditStatesChanged;
            _VisibleNavigationStates.CollectionChanged += VisibleNavigationStatesChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns true if <see cref="VisibleEditStates"/> contains all
        /// EditStates
        /// </summary>
        public bool ShowAllEditStates
        {
            get
            {
                return _ShowAllEditStates;
            }
            private set
            {
                _ShowAllEditStates = value;
                OnPropertyChanged(() => ShowAllEditStates);
            }
        }

        /// <summary>
        /// Returns true of <see cref="VisibleNavigationStates"/> contains all
        /// Record Navigation actions
        /// </summary>
        public bool ShowAllNavigationStates
        {
            get
            {
                return _ShowAllNavigationStates;
            }
            private set
            {
                _ShowAllNavigationStates = value;
                OnPropertyChanged(() => ShowAllNavigationStates);
            }
        }

        public bool ShowNavigationBar
        {
            get
            {
                return _ShowNavigationBar;
            }
            set
            {
                _ShowNavigationBar = value;
                OnPropertyChanged(() => ShowNavigationBar);
            }
        }

        public IList<EditState> VisibleEditStates
        {
            get
            {
                return _VisibleEditStates;
            }
        }

        public IList<RecordNavigation> VisibleNavigationStates
        {
            get
            {
                return _VisibleNavigationStates;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        protected virtual void OnPropertyChanged<T, T2>(Expression<Func<T, T2>> expression)
        {
            OnPropertyChanged(BindingHelper.Name(expression));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            OnPropertyChanged(BindingHelper.Name(expression));
        }

        void VisibleEditStatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_VisibleEditStates.ToList().Any(state => _VisibleEditStates.Count(duplicate => state == duplicate) > 1))
            {
                throw new ArgumentException("An element with the same key already exists");
            }

            ShowAllEditStates = _VisibleEditStates.Count == _AllEditStates.Count;

            OnPropertyChanged(() => VisibleEditStates);
        }

        void VisibleNavigationStatesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_VisibleNavigationStates.ToList().Any(state => _VisibleNavigationStates.Count(duplicate => state == duplicate) > 1))
            {
                throw new ArgumentException("An element with the same key already exists");
            }

            ShowAllNavigationStates = _VisibleNavigationStates.Count == _AllNavigationStates.Count;

            OnPropertyChanged(() => VisibleNavigationStates);
        }

    }
}