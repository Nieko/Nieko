using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nieko.Infrastructure.ViewModel
{
    /// <summary>
    /// ViewModel notifying of property changes and disposal. Also provides
    /// post-instantiation initialization
    /// </summary>
    public interface IViewModelBase : INotifyPropertyChanged, INotifyDisposing
    {
        /// <summary>
        /// ViewModels are initialized after View-ViewModel resolution
        /// and pre-initialization setup of UI Thread objects
        /// </summary>
        void Load();
    }
}
