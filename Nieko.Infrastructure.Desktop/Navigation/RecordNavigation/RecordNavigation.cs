using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public enum RecordNavigation
    {
        /// <summary>
        /// No navigation action
        /// </summary>
        None,
        /// <summary>
        /// To first item
        /// </summary>
        First,
        /// <summary>
        /// To previous item
        /// </summary>
        Previous,
        /// <summary>
        /// To next item
        /// </summary>
        Next,
        /// <summary>
        /// To the last item
        /// </summary>
        Last,
        /// <summary>
        /// To the index indicated
        /// </summary>
        Position,
        /// <summary>
        /// Display current number of items
        /// </summary>
        Count,
        /// <summary>
        /// Refresh current item
        /// </summary>
        Current
    }

}