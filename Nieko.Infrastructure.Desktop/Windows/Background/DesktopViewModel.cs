using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.ViewModel;
using Nieko.Infrastructure.Security;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Infrastructure.Windows.Background
{
    /// <summary>
    /// Default home screen.
    /// </summary>
    public class DesktopViewModel : ViewModelBase, IDesktopViewModel
    {
        private static object _Lock = new object();

        protected IStartupNotifier StartupNotifier { get; set; }
        protected IApplicationDetails ApplicationDetails { get; set; }

        protected static bool MenusCreated { get; private set; }

        /// <summary>
        /// Displayed in the middle of the desktop
        /// </summary>
        public virtual string ApplicationName
        {
            get
            {
                return Get(() => ApplicationName);
            }
            protected set
            {
                Set(() => ApplicationName, value);
            }
        }

        public DesktopViewModel(IStartupNotifier startupNotifier, IApplicationDetails applicationDetails, IViewNavigator regionNavigator, IMenuBarManager menuBarManager)
            : base()
        {
            StartupNotifier = startupNotifier;
        
            ApplicationDetails = applicationDetails;

            if (!MenusCreated)
            {
                MenusCreated = true;
                regionNavigator.EnqueueUIWork(() =>
                    {
                        menuBarManager.Create(@"_File\Save To Xml", EndPoint.Root, o => regionNavigator.SaveMainRegionDataToXml());
                    });
            }
        }

        static DesktopViewModel()
        {
            MenusCreated = false;
        }

        public override void Load()
        {
            lock (_Lock)
            {
                // Only process start-up notifications once
                if (StartupNotifier.Finished)
                {
                    return;
                }
                ApplicationName = ApplicationDetails.Title;
                StartupNotifier.Request();
                StartupNotifier.Process();
            }
        }
    }
}
