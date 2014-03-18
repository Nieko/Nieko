using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using Nieko.Infrastructure.ComponentModel;
using Nieko.Infrastructure.Data;

namespace Nieko.Infrastructure.Windows.Data
{
    /// <summary>
    /// Specialized ModelView functionality for memberships
    /// </summary>
    public interface IMembershipProviderLineItem : IEditableMirrorObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Remove action UI Controls visibility
        /// </summary>
        Visibility RemoveVisibility { get; set;  }
    }
}
