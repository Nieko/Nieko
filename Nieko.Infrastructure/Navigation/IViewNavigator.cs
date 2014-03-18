using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Collections;
using System.ComponentModel;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Navigation
{
    /// <summary>
    /// Manages loading and unloading of screens into the main view area, including
    /// execution of modal actions with UI notification and providing access to UI dependant
    /// Dialogs.
    /// </summary>
    /// <remarks>
    /// Is a facade around asynchronous execution and access to the UI and non-UI threads
    /// </remarks>
    public interface IViewNavigator
    {
        /// <summary>
        /// Raises an event with data about what views are being navigated to and from 
        /// </summary>
        event EventHandler<NavigationEventArgs>  Navigating;
        /// <summary>
        /// All multi-view regions (i.e. with tabs)
        /// </summary>
        ISet<string> KeptAliveRegions { get; }
        /// <summary>
        /// UI dependant dialogs
        /// </summary>
        IDialogs Dialogs { get; }
        /// <summary>
        /// Removes all Views from a region
        /// </summary>
        /// <param name="regionName">Region to clear</param>
        /// <returns>True if successful</returns>
        bool ClearRegion(string regionName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        bool NavigateTo(EndPoint endPoint);
        void EnqueueUIWork(Action work);
        void EnqueuePostLayoutWork(Action work);
        void SaveMainRegionDataToXml();
        void ExecuteModal(Action<object, DoWorkEventArgs> work, string message);
        void ExecuteModal(Action<object, DoWorkEventArgs> work, Action<object, RunWorkerCompletedEventArgs> finish, string message);
        void WaitModal(Func<bool> endCondition, string message);
        void Shutdown();
    }
}
