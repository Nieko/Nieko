using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Nieko.Modules.Navigation.RecordNavigator
{
    public class RecordFilterViewModel
    {
        public RecordFilter Filter { get; private set; }

        public ICommand DeleteCommand { get; internal set; }

        public RecordFilterViewModel(RecordFilter filter)
        {
            Filter = filter;
        }
    }
}
