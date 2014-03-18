using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ViewModel;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Specialized ViewModel for holding a Form Suite
    /// </summary>
    /// <typeparam name="T">Form Suite</typeparam>
    public interface ISuiteFormViewModel<T> : IViewModelBase 
        where T : FormsSuiteBase<T> 
    {
        /// <summary>
        /// Form Suite
        /// </summary>
        T Suite { get; set; }
    }
}
