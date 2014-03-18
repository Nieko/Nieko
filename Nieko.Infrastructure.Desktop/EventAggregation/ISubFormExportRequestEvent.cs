using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Used to request exportable version of <seealso cref="ISharedDataReport"/>
    /// </summary>
    public interface ISubFormExportRequestEvent : IInfrastructureEvent
    {
        /// <summary>
        /// EndPoint of report page data is requested of
        /// </summary>
        EndPoint SubPage { get; }
        /// <summary>
        /// Report page data
        /// </summary>
        object Data { get; set; }
        /// <summary>
        /// Sets report page EndPoint for event
        /// </summary>
        /// <param name="subPage"></param>
        void Initialize(EndPoint subPage);
    }
}
