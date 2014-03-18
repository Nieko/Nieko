using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Start-up infrastructure event signalling that Modules have been registered but that
    /// no start-up infrastructure events have concluded (i.e. plug-in system has not begun resolving plug-in instances)
    /// <seealso cref="Nieko.Prism.Unity.NiekoBootstrapper.PublishStartupEvents"/>
    /// </summary>
    public interface IModulesInitializedEvent : IInfrastructureEvent
    {
    }
}
