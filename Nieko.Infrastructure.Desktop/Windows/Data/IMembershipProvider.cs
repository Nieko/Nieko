using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Specialized Persisted View for membership editing
    /// </summary>
    public interface IMembershipProvider : IPersistedView
    {
        /// <summary>
        /// Raised when items are (re)loaded
        /// </summary>
        event EventHandler Reset; 
        /// <summary>
        /// Member to add if <see cref="AddCommand"/> executed (clicked)
        /// </summary>
        object NewItem { get; set; }
        /// <summary>
        /// Remaining unused memberships that may be added
        /// </summary>
        object Options { get; }
        /// <summary>
        /// Add membership to <see cref="NewItem"/>
        /// </summary>
        ICommand AddCommand { get; }
        /// <summary>
        /// Indicates if UI Control(s) to add a new membership be enabled
        /// </summary>
        bool AddEnabled { get; }
        /// <summary>
        /// Remove selected membership
        /// </summary>
        ICommand RemoveCommand { get; }
    }
}