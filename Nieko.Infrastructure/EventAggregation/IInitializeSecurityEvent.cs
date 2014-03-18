using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Security initialization occurs when the system is ready, but gets
    /// priority over all other activity that occurs at that time (i.e. anything that happens
    /// during the IApplicationInitializedEvent)
    /// <seealso cref="Nieko.Prism.Unity.NiekoBootstrapper.PublishStartupEvents"/>
    /// </summary>
    public interface IInitializeSecurityEvent : IInfrastructureEvent
    {
    }
}
