using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Start-up infrastructure event that essentially indicates that all other 
    /// infrastructure events have concluded <seealso cref="Nieko.Prism.Unity.NiekoBootstrapper.PublishStartupEvents"/>
    /// </summary>
    public interface IApplicationInitializedEvent : IInfrastructureEvent
    {
    }
}
