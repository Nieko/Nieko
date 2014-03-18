using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Signals to the IPluginFinder implementation to begin resolving plug-in instances
    /// <seealso cref="Nieko.Prism.Unity.NiekoBootstrapper.PublishStartupEvents"/>
    /// </summary>
    public interface IInitializePluginFrameworkEvent : IInfrastructureEvent
    {
    }
}
