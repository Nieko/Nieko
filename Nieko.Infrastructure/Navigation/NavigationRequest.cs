using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Contains data relating to a request to navigate a destination
    /// EndPoint
    /// </summary>
    public class NavigationRequest : INavigateRequest
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

        /// <summary>
        /// Intended destination
        /// </summary>
        public EndPoint Destination { get; private set; }

        /// <summary>
        /// Region whose view contents will be altered
        /// </summary>
        public string RegionName { get; private set; }

        /// <summary>
        /// Navigation manager
        /// </summary>
        public IViewNavigator RegionNavigator { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destination">Intended destination</param>
        /// <param name="regionName">Region whose view contents will be altered</param>
        /// <param name="regionNavigator">Navigation manager</param>
        public NavigationRequest(EndPoint destination, string regionName, IViewNavigator regionNavigator)
        {
            Destination = destination;
            RegionName = regionName;
            RegionNavigator = regionNavigator;
        }
    }
}
