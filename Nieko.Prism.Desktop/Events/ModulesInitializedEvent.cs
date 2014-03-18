using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Prism.Events
{
    public class ModulesInitializedEvent : CompositePresentationEvent<ModulesInitializedEvent>, IModulesInitializedEvent
    {
    }
}
