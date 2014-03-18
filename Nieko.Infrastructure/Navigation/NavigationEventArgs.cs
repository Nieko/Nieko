using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Holds data about what views are being navigated to and from 
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        public IEnumerable<object> OldViews { get; private set; }

        public EndPoint Destination { get; private set; }

        public object NewView { get; private set; }

        public NavigationEventArgs(IEnumerable<object> oldViews, EndPoint destination, object newView)
        {
            OldViews = oldViews;
            Destination = destination;
            NewView = newView;
        }
    }
}
