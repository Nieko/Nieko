using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Represents a suite of interchangeable forms 
    /// </summary>
    /// <remarks>
    /// Only one form in the suite is visible at a time, displayed in 
    /// the <see cref="FormRegion"/> region. A parent UI Control 
    /// controls tab navigation between the various forms
    /// </remarks>
    public interface IFormsSuite : INotifyDisposing
    {
        string FormRegion { get; }
        int SelectedTabIndex { get; set; }
        IList<ISuiteForm> Forms { get; }
        bool ShowExportData { get; }
        ICommand ExportData { get; }
    }
}
