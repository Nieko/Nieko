using Nieko.Infrastructure;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordSearchViewModel : NotifyPropertyChangedBase
    {
        private IDataNavigator _Navigator;
        private ObservableCollection<RecordFilterGroupViewModel> _FilterGroups;
        private IWeakEventRouter _RecordSearchRouter;

        public RecordSearch Search { get; private set; }

        public ICommand AddGroupCommand { get; private set; }

        public ICommand Apply { get; private set; }

        public ICommand Cancel { get; private set; }

        public ICommand Clear { get; private set; }

        public ListCollectionView FilterGroups { get; private set; }

        public RecordSearchViewModel(RecordSearch search, IDataNavigator navigator, Action closeAction)
        {
            _Navigator = navigator;
            Search = search;

            Apply = new RelayCommand(() =>
            {
                Search.Apply();
                closeAction();
            });

            Cancel = new RelayCommand(closeAction);

            Clear = new RelayCommand(() =>
            {
                Search.Take = 0;
                _FilterGroups.Clear();
            });
        }

        public void Initialize()
        {
            _RecordSearchRouter = WeakEventRouter.CreateInstance(this,
                Search,
                () => default(PropertyChangedEventArgs),
                (p, d) => p.PropertyChanged += d.Handler,
                (p, d) => p.PropertyChanged -= d.Handler,
                (s, p, a) =>
                {
                    if (a.PropertyName == BindingHelper.Name((RecordSearch rs) => rs.IsAdvancedMode))
                    {
                        foreach (var group in _FilterGroups)
                        {
                            group.ShowOrHeading = p.IsAdvancedMode && group.Group.Ordinal > 1 ?
                                Visibility.Visible :
                                Visibility.Collapsed;
                        }
                    }
                });

            _FilterGroups = new ObservableCollection<RecordFilterGroupViewModel>();
            FilterGroups = (ListCollectionView)CollectionViewSource.GetDefaultView(_FilterGroups);

            foreach (var group in Search.FilterGroups
                .Cast<RecordFilterGroup>())
            {
                AddGroup(new RecordFilterGroupViewModel(group, Search.BaseType));
            }

            AddGroupCommand = new RelayCommand(AddGroup);

            if (!Search.FilterGroups.Any())
            {
                AddGroup();
            }
        }

        private void AddGroup()
        {
            var nextOrdinal = _FilterGroups.Any() ?
                _FilterGroups.Max(lig => lig.Group.Ordinal) + 1 :
                1;

            var newItem = new RecordFilterGroupViewModel(new RecordFilterGroup()
            {
                Ordinal = nextOrdinal
            },
                Search.BaseType);

            Search.FilterGroups.Add(newItem.Group);
            AddGroup(newItem);
        }

        private void AddGroup(RecordFilterGroupViewModel group)
        {
            group.RemoveFilterGroup = new RelayCommand(() =>
            {
                _FilterGroups.Remove(group);
                Search.FilterGroups.Remove(group.Group);
            });

            FilterGroups.AddNewItem(group);
            FilterGroups.CommitNew(); 
            group.AddNewFilter();
        }
    }
}
