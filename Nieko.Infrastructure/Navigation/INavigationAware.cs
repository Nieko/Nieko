using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Provides functionality on ViewModels to listen for and
    /// potentially cancel navigation requests
    /// </summary>
    public interface INavigationAware
    {
        void NavigationRequested(NavigationRequest request);
        void CloseRequested(ExitRequest request);
    }
}
