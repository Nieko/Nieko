using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordFilterGroup : NotifyPropertyChangedBase, IRecordFilterGroup
    {
        public int Ordinal
        {
            get
            {
                return Get(() => Ordinal);
            }
            set
            {
                Set(() => Ordinal, value);
            }
        }

        public IList<IRecordFilter> Filters { get; private set; }

        public RecordFilterGroup()
        {
            Filters = new ObservableCollection<IRecordFilter>();
        }
    }
}
