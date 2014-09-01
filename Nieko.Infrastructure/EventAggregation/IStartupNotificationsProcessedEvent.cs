using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Announces that startup notifications have finished and subsequent events may proceed
    /// </summary>
    public interface IStartupNotificationsProcessedEvent : IInfrastructureEvent
    {
    }
}
