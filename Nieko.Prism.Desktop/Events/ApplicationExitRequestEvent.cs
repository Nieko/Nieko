using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Prism.Events
{
    public class ApplicationExitRequestEvent : CompositePresentationEvent<ApplicationExitRequestEvent>, IApplicationExitRequestEvent
    {
        private bool _Cancel;

        public bool Cancel
        {
            get
            {
                return _Cancel;
            }
            set
            {
                _Cancel = _Cancel || value;
            }
        }
    }
}
