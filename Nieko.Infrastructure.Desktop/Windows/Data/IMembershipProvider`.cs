using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Generic Membership Provider for a ModelView <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">ModelView of a membership</typeparam>
    public interface IMembershipProvider<T> : IPersistedView<T>, IMembershipProvider
        where T : class, IMembershipProviderLineItem, new()
    {
        /// <summary>
        /// Type checked collection of <seealso cref="IMembershipProvider.Options"/>
        /// </summary>
        ObservableCollection<T> AvailableMemberships { get; }
        /// <summary>
        /// Type checked <seealso cref="IMembershipProvider.NewItem"/>
        /// </summary>
        new T NewItem { get; set; }
        /// <summary>
        /// Method to check if a remove action is currently allowed
        /// </summary>
        Func<IMembershipProvider<T>, bool> RemoveAllowed { get; set; }
    }
}
