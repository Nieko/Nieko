using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.EventAggregation;
using Microsoft.Practices.Prism.Events;

namespace Nieko.Prism.Events
{
    public class InitialActionRequestEvent : CompositePresentationEvent<InitialActionRequestEvent>, IInitialActionRequestEvent
    {
        public Action InitialAction { get; private set; }

        public void SetInitialAction(Action action)
        {
            if (InitialAction != null)
            {
                throw new InvalidOperationException("Initial Action has already been set"); 
            }

            InitialAction = action;
        }
    }
}
