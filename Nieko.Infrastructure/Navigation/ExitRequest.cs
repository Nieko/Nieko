using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Represents a polite request to exit the application
    /// </summary>
    public class ExitRequest : INavigateRequest
    {
        private bool _Cancel;

        public bool Cancel
        {
            get
            {
                return _Cancel;
            }
            set
            {
                _Cancel = _Cancel | value;
            }
        }

        public IViewNavigator RegionNavigator { get; private set; }

        public ExitRequest(IViewNavigator regionNavigator)
        {
            RegionNavigator = regionNavigator;
        }
    }
}
