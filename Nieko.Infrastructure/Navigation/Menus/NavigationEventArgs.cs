using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.Menus
{
    public class NavigationEventArgs : EventArgs
    {
        public bool Cancel { get; set; }

        public NavigationEventArgs()
            : base()
        {
            Cancel = false;
        }

    }
}