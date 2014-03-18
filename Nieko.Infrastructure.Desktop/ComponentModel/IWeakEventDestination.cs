using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Facade for Weak Event handling mechanism provided by
    /// by a WeakEventRouter
    /// </summary>
    /// <remarks>
    /// Actual event handling is passed through to a delegate provided
    /// to the WeakEventRouter at construction
    /// </remarks>
    public interface IWeakEventDestination
    {
        void Handler(object sender, EventArgs args);
    }
}
