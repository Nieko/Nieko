using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure.ComponentModel;
using Microsoft.Practices.Prism.Events;

namespace Nieko.Prism.Events
{
    public class SubFormExportRequestEvent :  CompositePresentationEvent<SubFormExportRequestEvent>, ISubFormExportRequestEvent
    {
        private object _Data = null;
        public EndPoint SubPage { get; private set; }

        public object Data
        {
            get
            {
                return _Data;
            }
            set
            {
                if (_Data != null)
                {
                    throw new InvalidOperationException("Data has already been set");
                }

                _Data = value;
            }
        }

        public void Initialize(EndPoint subPage)
        {
            if (SubPage != null)
            {
                throw new InvalidOperationException("Event has already been initialized");
            }

            SubPage = subPage;
        }
    }
}
