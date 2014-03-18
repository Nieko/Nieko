using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Visible Modal Message Box buttons
    /// </summary>
    [Flags]
    public enum ModalMessageButton
    {
        Yes = 1 << 0,
        No = 1 << 1,
        Ok = 1 << 2,
        Cancel = 1 << 3
    }

    /// <summary>
    /// UI independent dialogs access
    /// </summary>
    // TODO add UI dependant file dialog (i.e. Open / Save etc) functionality
    public interface IDialogs
    {
        /// <summary>
        /// Creates a platform specific Wait Dialog
        /// </summary>
        /// <returns>Wait Dialog</returns>
        IWaitDialog CreateWaitDialog();
        /// <summary>
        /// Show untitled message with Ok button
        /// </summary>
        /// <param name="message">Text displayed</param>
        /// <returns>Button pressed</returns>
        ModalMessageButton ShowModalMessage(string message);
        /// <summary>
        /// Show untitled message
        /// </summary>
        /// <param name="message">Text displayed</param>
        /// <param name="buttons">Buttons to show</param>
        /// <returns>Button pressed</returns>
        ModalMessageButton ShowModalMessage(string message, ModalMessageButton buttons);
        /// <summary>
        /// Show message
        /// </summary>
        /// /// <param name="title">Dialog title</param>
        /// <param name="message">Text displayed</param>
        /// <param name="buttons">Buttons to show</param>
        /// <returns>Button pressed</returns>
        ModalMessageButton ShowModalMessage(string title, string message, ModalMessageButton buttons);
    }
}
