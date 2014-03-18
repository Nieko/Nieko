using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Windows
{
    /// <summary>
    /// Details of a Form to be displayed by an EndPoint, implemented
    /// by an MVVM Form
    /// </summary>
    public class ViewModelForm
    {
        /// <summary>
        /// EndPoint Form is associated to
        /// </summary>
        public EndPoint EndPoint { get; internal set; }

        /// <summary>
        /// Type of Presentation UI
        /// </summary>
        public Type ViewType { get; internal set; }

        /// <summary>
        /// Type of Presentation UI
        /// </summary>
        public Type ViewModelType { get; internal set; }

        /// <summary>
        /// Method to create an instance of the View
        /// </summary>
        public Func<object> ViewFactory { get; internal set; }

        /// <summary>
        /// Method to create an instance of the ViewModel
        /// </summary>
        public Func<object> ViewModelFactory { get; internal set; }

        /// <summary>
        /// Method to set the ViewModel for the View
        /// </summary>
        public Action<object, object> ViewModelSet { get; internal set; }

        /// <summary>
        /// Presentation Region in which to display the View
        /// </summary>
        public string RegionName { get; internal set; }

        internal ViewModelForm() {}
    }
}
