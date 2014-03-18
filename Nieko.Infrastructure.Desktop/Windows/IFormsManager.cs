using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Export;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Single instance Manager for EndPoint forms
    /// </summary>
    public interface IFormsManager
    {
        /// <summary>
        /// All Forms declared by their EndPoint
        /// </summary>
        IDictionary<EndPoint, ViewModelForm> FormsByEndPoint { get; }
        /// <summary>
        /// Attempt to navigate to the Form associated with the EndPoint.
        /// An exception is not raised if the EndPoint does not have an
        /// associated Form
        /// </summary>
        /// <param name="formEndPoint">EndPoint to navigate to</param>
        /// <returns>True if not cancelled</returns>
        bool Show(EndPoint formEndPoint);
        /// <summary>
        /// Attempt to navigate to the Form associated with the EndPoint and
        /// raise an exception if there is no associated Form
        /// </summary>
        /// <param name="formEndPoint">EndPoint to navigate to</param>
        /// <returns>True if not cancelled</returns>
        bool Show(EndPoint formEndPoint, bool throwOnFormMissing);
        /// <summary>
        /// Logs the exception, closes all Forms, displays an error message
        /// and returns to the initial application screen
        /// </summary>
        /// <param name="prefix">Message prefix</param>
        /// <param name="e">Exception</param>
        void HandleFormException(string prefix, Exception e);
    }
}
