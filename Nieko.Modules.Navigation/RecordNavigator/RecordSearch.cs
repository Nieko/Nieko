using Nieko.Infrastructure.Collections;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure.Windows.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordSearch : NotifyPropertyChangedBase, IRecordSearch
    {
        private ICollectionView _View;
        private ObservableRangeCollection<IRecordFilterGroup> _FilterGroups = new ObservableRangeCollection<IRecordFilterGroup>();
        private ObservableRangeCollection<SortDescription> _SortDescriptions = new ObservableRangeCollection<SortDescription>();

        public IList<IRecordFilterGroup> FilterGroups { get { return _FilterGroups; } }

        public IList<SortDescription> SortDescriptions { get { return _SortDescriptions; } }

        public Type BaseType { get; private set; }

        public IEntityPersistedView EntityDetails
        {
            get
            {
                return Get(() => EntityDetails);
            }
            set
            {
                Set(() => EntityDetails, value);
            }
        }

        public bool IsApplyingSearch
        {
            get
            {
                return Get(() => IsApplyingSearch);
            }
            set
            {
                Set(() => IsApplyingSearch, value, () =>
                    {
                        if(!value)
                        {
                            FilterGroups.Clear();
                            SortDescriptions.Clear();
                            Apply();
                        }
                        else
                        {
                            IsApplyingSearch = !IsClearedFilter;
                        }
                    });
            }
        }

        public bool IsAdvancedMode
        {
            get
            {
                return Get(() => IsAdvancedMode);
            }
            set
            {
                Set(() => IsAdvancedMode, value, ChangeMode);
            }
        }

        public long Take
        {
            get
            {
                return Get(() => Take);
            }
            set
            {
                Set(() => Take, value);
            }
        }

        private bool IsClearedFilter
        {
            get
            {
                return !(FilterGroups.Any(li => li.Filters.Any()) || SortDescriptions.Any() || Take > 0);
            }
        }

        public RecordSearch(ICollectionView view, Type baseType)
        {
            _View = view;
            BaseType = baseType;

            SetDefault(() => IsAdvancedMode, false);
            SetDefault(() => Take, 0);
            SetDefault(() => IsApplyingSearch, false);
        }

        public bool QuickFind<T>(Func<T, bool> filter) where T : class
        {
            IsApplyingSearch = true;
            _View.Filter = o =>  o is T && filter(o as T);

            return !_View.IsEmpty;
        }

        public void Apply()
        {
            if(IsClearedFilter)
            {
                _View.Filter = null;
                _View.SortDescriptions.Clear();
            }
            else
            {
                var predicate = BuildPredicate();
                
                _View.SortDescriptions.Clear();
                foreach(var sort in SortDescriptions)
                {
                    _View.SortDescriptions.Add(sort);
                }

                _View.Filter = o => predicate(o);
            }

            IsApplyingSearch = !IsClearedFilter;
        }

        private Func<object, bool> BuildPredicate()
        {
            var typeParameter = Expression.Parameter(BaseType, "");
            var objectParameter = Expression.Parameter(typeof(object), "o");
            BinaryExpression all = null;

            foreach(var group in FilterGroups)
            {
                BinaryExpression currentLine = null;

                foreach(var filter in group.Filters
                    .Cast<RecordFilter>())
                {
                    var lineAggregate = currentLine;
                    currentLine = filter.BuildPathExpression(typeParameter);
                    
                    if(lineAggregate != null)
                    {
                        currentLine = Expression.And(lineAggregate, currentLine);
                    }
                }

                var groupAggregate = all;

                if (groupAggregate != null)
                {
                    all = Expression.Or(groupAggregate, currentLine);
                }
                else
                {
                    all = currentLine;
                }
            }

            var filterMethod = Expression.Lambda(typeof(Func<,>).MakeGenericType(BaseType, typeof(bool)),
                all,
                typeParameter)
                .Compile();
            var castMethod = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(object), BaseType),
                Expression.Convert(objectParameter, BaseType),
                objectParameter)
                .Compile();
            Func<object, bool> predicate = o => (bool)filterMethod.DynamicInvoke(castMethod.DynamicInvoke(o));

            return predicate;
        }

        private void ChangeMode()
        {
            if(!IsAdvancedMode)
            {
                if (FilterGroups.Count > 1)
                {
                    var firstGroup = FilterGroups[0];

                    foreach(var removedGroup in FilterGroups
                        .Where(li => li != firstGroup)
                        .ToList())
                    {
                        FilterGroups.Remove(removedGroup);
                    }
                }
            }
        }
    }
}
