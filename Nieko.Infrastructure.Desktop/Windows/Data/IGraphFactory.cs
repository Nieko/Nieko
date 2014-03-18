using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Data;
using Nieko.Infrastructure.ComponentModel.ViewModelling;
using Nieko.Infrastructure.ComponentModel;
using System.Windows.Data;
using Nieko.Infrastructure.Navigation.RecordNavigation;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Start of process to manufacture objects to manage a 
    /// ModelView graph
    /// </summary>
    /// <remarks>
    /// Acts as a fluent factory facade over a number of complex build
    /// actions and allowing the construction of as many graph tiers / leaves as required
    /// </remarks>
    public interface IGraphFactory
    {
        /// <summary>
        /// Lifetime provider for objects managing graph
        /// </summary>
        /// <param name="owner">Lifetime provider</param>
        /// <returns>Next step in factory</returns>
        IOwnedGraphFactory OwnedBy(INotifyDisposing owner);
    }
}
