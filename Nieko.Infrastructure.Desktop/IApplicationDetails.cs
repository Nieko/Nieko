using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Windows;

namespace Nieko.Infrastructure
{
    /// <summary>
    /// Application description and start-up screen details
    /// </summary>
    public interface IApplicationDetails
    {
        string Title { get; }
        string HelpAboutText { get; }
        bool ShowMenuBarAtStartup { get; }
        int InitialHeight { get; }
        int InitialWidth { get; }
        /// <summary>
        /// MVVM details of Home screen. The start-up form is responsible for 
        /// displaying and processing <seealso cref="IStartupNotifier"/>
        /// messages
        /// </summary>
        StartupFormDetails StartupForm { get; }
    }
}
