using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Implemented by the user interface providing visual control
    /// of Data Navigation and editing
    /// </summary>
    public interface IDataNavigatorView
    {
        /// <summary>
        /// ViewModel implementation for the visual control
        /// </summary>
        IDataNavigatorViewModel ViewModel { get; set; }
    }
}
