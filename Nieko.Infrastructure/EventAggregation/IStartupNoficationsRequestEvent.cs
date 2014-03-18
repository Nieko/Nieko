using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Requests any messages or actions that need to processed at start-up.
    /// </summary>
    /// <remarks>
    /// Notifications may be critical in which case the application will be shutdown
    /// </remarks>
    public interface IStartupNotificationsRequestEvent : IInfrastructureEvent
    {
        Queue<StartupNotification> CriticalNotifications { get; }
        Queue<StartupNotification> NonCriticalNotifications { get; }
    }
}
