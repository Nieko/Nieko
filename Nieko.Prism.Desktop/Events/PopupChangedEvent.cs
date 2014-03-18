using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Prism.Events
{
    public class PopupChangedEvent : CompositePresentationEvent<PopupChangedEvent>, IPopupChangedEvent
    {
        private bool _ShowSet = false;

        public bool Show { get; private set; }

        public void SetShow(bool show)
        {
            if (_ShowSet)
            {
                throw new InvalidOperationException("Show already set");
            }

            _ShowSet = true;
            Show = show;
        }
    }
}
