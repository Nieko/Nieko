using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Represents a single Form in a Suite
    /// </summary>
    public interface ISuiteForm
    {
        /// <summary>
        /// Display order relative to the other forms
        /// in the suite
        /// </summary>
        int Ordinal { get; }
        /// <summary>
        /// Caption text used in any UI display for this form
        /// </summary>
        string Caption { get; }
        /// <summary>
        /// Method to display this form
        /// </summary>
        Action ShowAction { get; }
    }
}
