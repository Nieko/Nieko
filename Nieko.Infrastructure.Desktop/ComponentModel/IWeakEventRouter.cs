using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Facade around an event router to allow easy access to
    /// manual Subscription Cancellation
    /// </summary>
    public interface IWeakEventRouter : IWeakEventListener
    {
        void CancelSubscription();
    }
}
