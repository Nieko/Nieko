using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Represents a UI Presentation View with a strongly-typed member
    /// for the ViewModel
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IViewBase<TViewModel>
    {
        /// <summary>
        /// MVVM ViewModel for this View
        /// </summary>
        TViewModel ViewModel { get; set; }
    }
}
