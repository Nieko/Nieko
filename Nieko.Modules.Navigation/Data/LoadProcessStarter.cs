using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using System.ComponentModel;
using Nieko.Infrastructure.Windows.Data;

namespace Nieko.Modules.Navigation.Data
{
    internal class LoadProcessStarter : INotifyModelViewGraphNodeLoaded
    {
        public event EventHandler Loaded = delegate { };

        public void Start()
        {
            Loaded(this, EventArgs.Empty);
        }
    }
}
