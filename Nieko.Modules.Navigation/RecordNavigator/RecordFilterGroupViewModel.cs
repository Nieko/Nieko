using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordFilterGroupViewModel: NotifyPropertyChangedBase
    {
        private ObservableCollection<RecordFilterViewModel> _Filters;
        private Type _BaseType;

        public RecordFilterGroup Group { get; private set; }

        public Visibility ShowOrHeading
        {
            get
            {
                return Get(() => ShowOrHeading);
            }
            set
            {
                Set(() => ShowOrHeading, value);
            }
        }

        public ListCollectionView Filters { get; private set; }

        public ICommand AddNewFilterCommand { get; private set; }

        public ICommand RemoveFilterGroup { get; internal set; }

        public RecordFilterGroupViewModel(RecordFilterGroup group, Type baseType)
        {
            _BaseType = baseType;
            Group = group;
            ShowOrHeading = Group.Ordinal == 1 ?
                Visibility.Collapsed :
                Visibility.Visible;

            _Filters = new ObservableCollection<RecordFilterViewModel>();
            Filters = (ListCollectionView)CollectionViewSource.GetDefaultView(_Filters);

            foreach (var filter in group.Filters
                .Cast<RecordFilter>())
            {
                AddNewFilter(filter);
            };

            AddNewFilterCommand = new RelayCommand(AddNewFilter);
        }

        internal void AddNewFilter()
        {
            var filter = new RecordFilter();
            filter.Initialize(_BaseType);

            AddNewFilter(filter);
            Group.Filters.Add(filter);
        }

        internal void AddNewFilter(RecordFilter filter)
        {
            var line = new RecordFilterViewModel(filter);

            line.DeleteCommand = new RelayCommand(() =>
            {
                _Filters.Remove(line);
                Group.Filters.Remove(line.Filter);
            });

            _Filters.Add(line);
        }
    }
}
