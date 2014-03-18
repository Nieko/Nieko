using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Configures the user interface for Data Navigation and notifies
    /// screen regions that the Data Navigator UI should be removed via
    /// <seealso cref="INotifyDisposing"/>
    /// </summary>
    public interface IUIConfig : INotifyDisposing
    {
        /// <summary>
        /// Whether or not a UI should even be created at all. False precludes
        /// use of the remaining members
        /// </summary>
        bool CreateView { get; }
        /// <summary>
        /// UI Region in which to create Navigation Control
        /// </summary>
        string Region { get; }
        /// <summary>
        /// Visibility Provider provided to Data Navigator when it
        /// is first created
        /// </summary>
        INavigatorVisibilityProvider VisibilityProvider { get; }
    }
}



