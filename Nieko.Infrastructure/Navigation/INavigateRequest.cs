using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Represents a request to navigate away from the current form
    /// </summary>
    public interface INavigateRequest
    {
        bool Cancel { get; set; }
        IViewNavigator RegionNavigator { get; }
    }
}
