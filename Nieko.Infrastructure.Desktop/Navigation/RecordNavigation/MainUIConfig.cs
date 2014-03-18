using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.Navigation.RecordNavigation
{
    /// <summary>
    /// Data Navigation UI Configuration to show a navigation
    /// bar in the standard region set aside for a top-tier 
    /// navigation toolbar
    /// </summary>
    public sealed class MainUIConfig : IUIConfig
    {
        private bool _IsDisposing;

        public bool CreateView { get; private set; }

        public string Region { get; private set; }

        public INavigatorVisibilityProvider VisibilityProvider { get; private set; }

        public MainUIConfig(INotifyDisposing owner)
        {
            CreateView = true;
            Region = CoreRegionNames.BottomRegion;
            VisibilityProvider = new SimpleVisibilityProvider();

            EventHandler ownerDisposing = null;

            ownerDisposing = (sender, args) =>
            {
                Dispose();
                owner.Disposing -= ownerDisposing;
            };

            owner.Disposing += ownerDisposing;
        }

        public IUIConfig AtRegion(string regionName)
        {
            Region = regionName;

            return this;
        }

        public IUIConfig UsingVisibilityProvider(INavigatorVisibilityProvider visibilityProvider)
        {
            VisibilityProvider = visibilityProvider;

            return this;
        }

        public event EventHandler Disposing;

        public void Dispose()
        {
            if (_IsDisposing)
            {
                return;
            }

            _IsDisposing = true;

            var handler = Disposing;

            if (handler != null)
            {
                handler(this, EventArgs.Empty); 
            }

            VisibilityProvider = null;
        }
    }
}
