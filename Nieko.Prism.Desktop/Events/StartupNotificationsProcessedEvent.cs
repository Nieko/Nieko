using Microsoft.Practices.Prism.Events;
using Nieko.Infrastructure.EventAggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Prism.Events
{
    public class StartupNotificationsProcessedEvent : CompositePresentationEvent<StartupNotificationsProcessedEvent>, IStartupNotificationsProcessedEvent 
    {
    }
}
