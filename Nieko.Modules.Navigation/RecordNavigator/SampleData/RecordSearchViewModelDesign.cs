#if DEBUG
using Nieko.Infrastructure;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordSearchViewModelDesign : RecordSearchViewModel
    {
        private static RecordSearch _Search;

        public RecordSearchViewModelDesign(IDataNavigator dataNavigator) : base(_Search, dataNavigator, () => { }) { }

        public RecordSearchViewModelDesign() : this(new DataNavigatorViewModelSample()) { }

        static RecordSearchViewModelDesign()
        {
            _Search = new RecordSearch(null, typeof(RecordSearchBaseItem))
            {
                IsAdvancedMode = true
            };
            
            _Search.FilterGroups.Add(new RecordFilterGroup()
            {
                Ordinal = 1
            });

            _Search.FilterGroups[0].Filters.Add(new RecordFilter()
            {
                Comparison = "=",
                Path = "Name",
                Filter = "Sprocket"
            });

            _Search.FilterGroups[0].Filters.Add(new RecordFilter()
            {
                Comparison = "!=",
                Path = "Units",
                Filter = "0"
            });

            _Search.FilterGroups.Add(new RecordFilterGroup()
            {
                Ordinal = 2
            });

            _Search.FilterGroups[1].Filters.Add(new RecordFilter()
            {
                Comparison = "=",
                Path = "Name",
                Filter = "Widget"
            });

            _Search.SortDescriptions.Add(new SortDescription(BindingHelper.Name((RecordSearchBaseItem bi) => bi.Units), ListSortDirection.Descending));
            _Search.Take = 15;
        }
    }
}
#endif
