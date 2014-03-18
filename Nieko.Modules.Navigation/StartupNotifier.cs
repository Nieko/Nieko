using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.Navigation;
using Nieko.Infrastructure.EventAggregation;

namespace Nieko.Modules.Navigation
{
    public class StartupNotifier : IStartupNotifier
    {
        private readonly IInfrastructureEventAggregator _EventAggregator;
        private readonly IViewNavigator _RegionNavigator;
        private IStartupNotificationsRequestEvent _RequestEvent;

        public bool Finished { get; private set; }

        public void Request()
        {
            _RequestEvent = _EventAggregator.CreateEvent<IStartupNotificationsRequestEvent>();  
            _EventAggregator.Publish<IStartupNotificationsRequestEvent>(_RequestEvent);    
        }

        public StartupNotifier(IInfrastructureEventAggregator eventAggregator, IViewNavigator regionNavigator)
        {
            _EventAggregator = eventAggregator;
            _RegionNavigator = regionNavigator;
        }

        public void Process()
        {
            if (_RequestEvent.CriticalNotifications.Count != 0)
            {
                ShowNotification(_RequestEvent.CriticalNotifications.Dequeue());
                System.Windows.Application.Current.Shutdown();
            }

            foreach (var notification in _RequestEvent.NonCriticalNotifications)
            {
                ShowNotification(notification);
            }

            Finished = true;

            var initialActionRequest = _EventAggregator.CreateEvent<IInitialActionRequestEvent>();
            _EventAggregator.Publish(initialActionRequest);

            if (initialActionRequest.InitialAction != null)
            {
                initialActionRequest.InitialAction();
            }
        }

        private void ShowNotification(StartupNotification notification)
        {
            if (!notification.RunInBackground)
            {
                _RegionNavigator.Dialogs.ShowModalMessage(notification.Message);
            }

            if (notification.Callback != null)
            {
                if (notification.RunInBackground)
                {
                    _RegionNavigator.ExecuteModal((sender, args) => notification.Callback(), notification.Message);
                }
                else
                {
                    notification.Callback();
                }
            }
        }

    }
}
