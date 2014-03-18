using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation.RecordNavigation;
using Nieko.Infrastructure;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    public class UIConfig : IUIConfig
    {
        private bool _IsDiposing;

        public event EventHandler Disposing;

        public bool CreateView { get; private set; }

        public string Region { get; private set; }

        public INavigatorVisibilityProvider VisibilityProvider { get; private set; }

        /// <summary>
        /// General configuration to display no UI
        /// </summary>
        public static IUIConfig NoView { get; private set; }

        /// <summary>
        /// General configuration to display the UI in the <seealso cref="CoreRegionNames.BottomRegion"/>
        /// using a <seealso cref="SimpleVisiblityProvider"/>
        /// </summary>
        public static IUIConfig DefaultView { get; private set; }

        public UIConfig(INotifyDisposing owner) : this()
        {
            _IsDiposing = false;

            EventHandler ownerDisposing = null;

            ownerDisposing = (sender, args) =>
                {
                    Dispose();
                    owner.Disposing -= ownerDisposing;
                };

            owner.Disposing += ownerDisposing;
        }

        private UIConfig()
        {
            Region = CoreRegionNames.BottomRegion;
            CreateView = true;
            VisibilityProvider = new SimpleVisibilityProvider();
        }

        static UIConfig()
        {
            var noView = new UIConfig();
            noView.CreateView = false;
            noView.Region = string.Empty;

            NoView = noView;

            DefaultView = new UIConfig();
        }

        /// <summary>
        /// Fluent interface for configuring the UI region for 
        /// a new instance. 
        /// </summary>
        /// <remarks>
        /// <seealso cref="IUIConfig "/> does not expose
        /// members to change its configuration 
        /// </remarks>
        /// <param name="regionName">Region to display UI in</param>
        /// <returns>Current instance</returns>
        public UIConfig AtRegion(string regionName)
        {
            Region = regionName;
            return this;
        }

        /// <summary>
        /// Fluent interface for changing the visibility provider
        /// for a new instance. 
        /// </summary>
        /// <remarks>
        /// <seealso cref="IUIConfig "/> does not expose
        /// members to change its configuration 
        /// </remarks>
        /// <param name="regionName">Visibility provider</param>
        /// <returns>Current instance</returns>
        public UIConfig UsingVisibilityProvider(INavigatorVisibilityProvider visibilityProvider)
        {
            VisibilityProvider = visibilityProvider;

            return this;
        }

        public void Dispose()
        {
            if (_IsDiposing)
            {
                return;
            }

            _IsDiposing = true;

            var handler = Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
