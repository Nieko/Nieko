using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// IPersistedView built using the IGraphFactory fluent interfaces
    /// </summary>
    public interface IConstructedPersistedView : IPersistedView
    {
        /// <summary>
        /// Root node IPersistedView of the entire graph tree
        /// </summary>
        IPersistedViewRoot Root { get; }
    }
}
