using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Notification message details given to an <seealso cref="IStartupNotificationsRequestEvent"/> event.
    /// </summary>
    public class StartupNotification
    {
        public string Message { get; private set; }
        public Action Callback { get; private set; }
        public bool RunInBackground { get; private set; }

        /// <summary>
        /// StartupNotification Constructor
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="callback">Action to perform</param>
        /// <param name="runInBackground">If true, shows message in dialog and runs call-back in background
        /// If false, displays modal message and execute call-back, if any</param>
        public StartupNotification(string message, Action callback, bool runInBackground)
        {
            if (runInBackground && (callback == null || string.IsNullOrEmpty(message)))
            {
                throw new ArgumentException("A display message and callback must be provided if the notification is to run in the background");
            }

            Message = message;
            Callback = callback;
            RunInBackground = runInBackground;
        }

        /// <summary>
        /// StartupNotification Constructor
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="callback">Action to perform</param>
        public StartupNotification(string message, Action callback) : this(message, callback, false) { }

        /// <summary>
        /// StartupNotification Constructor
        /// </summary>
        /// <param name="message">Message to display</param>
        public StartupNotification(string message) : this(message, null, false) { }
    }
}