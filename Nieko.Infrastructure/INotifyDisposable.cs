using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure
{
    /// <summary>
    /// <see cref="IDisposable"/> functionality with an additional
    /// event notifying when an object has begun disposal
    /// </summary>
    public interface INotifyDisposing : IDisposable
    {
        event EventHandler Disposing;
    }
}