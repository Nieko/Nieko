using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.ComponentModel;

namespace Nieko.Modules.Navigation
{
    internal class EndPointContextLifetimeManager
    {
        private object _LockObject = new object();
        private IWeakEventRouter _ViewChangedRouter;
        private Dictionary<object, EndPoint> _ContentEndPoints = new Dictionary<object, EndPoint>();

        public EndPointContextLifetimeManager(IViewNavigator viewNavigator)
        {
            _ViewChangedRouter = WeakEventRouter.CreateInstance(this,
                viewNavigator,
                () => default(NavigationEventArgs),
                (o, d) => o.Navigating += d.Handler,
                (o, d) => o.Navigating -= d.Handler,
                (s, p, a) => OnNavigation(a));
        }

        private void OnNavigation(NavigationEventArgs args)
        {
            lock (_LockObject)
            {
                EndPoint contentEndPoint;

                foreach (var oldView in args.OldViews)
                {
                    if (_ContentEndPoints.TryGetValue(oldView, out contentEndPoint))
                    {
                        contentEndPoint.MetaContext = null;
                        _ContentEndPoints.Remove(oldView);
                    }
                }

                if (args.NewView == null)
                {
                    _ContentEndPoints.Add(args.NewView, args.Destination);
                }
            }
        }
    }
}
