using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Raised by the IPluginFinder to indicate that all plug-in
    /// discovery at application start-up has been satisfied
    /// </summary>
    /// <remarks>
    /// Further use of IPluginFinder is entirely possible after the
    /// application has finished initializing but this event indicates
    /// that any plug-ins necessary for application initialization have
    /// been created
    /// </remarks>
    public interface IPluginsLoadedEvent : IInfrastructureEvent
    {
    }
}
