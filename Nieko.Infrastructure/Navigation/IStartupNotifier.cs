using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Raises and processes the IStartupNotificationsRequestEvent and IInitialActionRequestEvent
    /// Infrastructure events at application start up.
    /// </summary>
    public interface IStartupNotifier
    {
        /// <summary>
        /// Indicates start up events have already been published and processed
        /// </summary>
        bool Finished { get; }
        /// <summary>
        /// Publish start up events to request start up actions
        /// </summary>
        void Request();
        /// <summary>
        /// Process start up actions
        /// </summary>
        void Process();
    }
}