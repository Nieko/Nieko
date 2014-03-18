using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public enum EditState
    {
        /// <summary>
        /// No record displayed
        /// </summary>
        None,
        /// <summary>
        /// View an existing instance but not edit
        /// </summary>
        View,
        /// <summary>
        /// Edit a newly created instance
        /// </summary>
        New,
        /// <summary>
        /// Remove an existing instance
        /// </summary>
        Delete,
        /// <summary>
        /// Edit an existing instance
        /// </summary>
        Edit,
        /// <summary>
        /// Save new record or changes to an existing instance
        /// </summary>
        Save,
        /// <summary>
        /// Undo editing or creation of new instance
        /// </summary>
        Cancel
    }

}