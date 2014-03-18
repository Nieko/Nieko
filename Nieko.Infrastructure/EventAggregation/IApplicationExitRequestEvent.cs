using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Cancellable polite attempt to close the current application
    /// </summary>
    public interface IApplicationExitRequestEvent : IInfrastructureEvent
    {
        bool Cancel { get; set; }
    }
}
