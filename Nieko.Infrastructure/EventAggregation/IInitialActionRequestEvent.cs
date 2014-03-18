using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Raises a request from the Framework to the application for an initial action
    /// to perform once all start-up actions have occurred; i.e. the -last- thing for
    /// the Framework to do.
    /// </summary>
    /// <remarks>
    /// Only one Initial Action may be set by the application using the Framework. This
    /// event is raised when the initial desktop screen is first loaded
    /// </remarks>
    public interface IInitialActionRequestEvent : IInfrastructureEvent
    {
        Action InitialAction { get; }
        void SetInitialAction(Action action);
    }
}
