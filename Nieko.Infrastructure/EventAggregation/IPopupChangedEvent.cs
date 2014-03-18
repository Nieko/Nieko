using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Updates subscribers on the shown or not-shown status of the
    /// modal popup dialog.
    /// </summary>
    public interface IPopupChangedEvent : IInfrastructureEvent
    {
        bool Show { get; }
        void SetShow(bool show);
    }
}
