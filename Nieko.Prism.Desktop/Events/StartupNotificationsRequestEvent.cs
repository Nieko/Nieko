using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using Microsoft.Practices.Prism.Events;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Prism.Events
{
    public class StartupNotificationsRequestEvent : CompositePresentationEvent<StartupNotificationsRequestEvent>, IStartupNotificationsRequestEvent 
    {
        public Queue<StartupNotification> CriticalNotifications { get; private set; }
        public Queue<StartupNotification> NonCriticalNotifications { get; private set; }

        public StartupNotificationsRequestEvent()
        {
            CriticalNotifications = new Queue<StartupNotification>();
            NonCriticalNotifications = new Queue<StartupNotification>();
        }
    }
}
