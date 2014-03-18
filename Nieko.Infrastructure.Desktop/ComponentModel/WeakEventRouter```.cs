using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using l = System.Linq.Expressions;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class WeakEventRouter<TPublisher, TArgs, TSubscriber>
        where TPublisher : class
        where TArgs : EventArgs
    {
        internal static ListeningActions DefaultListeningActions = new ListeningActions();

        internal class ListeningActions
        {
            internal Action<TPublisher, IWeakEventDestination> StartListeningToAction { get; set; }

            internal Action<TPublisher, IWeakEventDestination> StopListeningToAction { get; set; }
        }

        internal class DefaultManager : WeakEventManagerBase<DefaultManager, TPublisher>, IWeakEventDestination
        {
            internal static ListeningActions ListeningActions { get; set; }

            protected override void StartListeningTo(TPublisher source)
            {
                ListeningActions.StartListeningToAction(source, this);
            }

            protected override void StopListeningTo(TPublisher source)
            {
                ListeningActions.StopListeningToAction(source, this);
            }

            static DefaultManager()
            {
                ListeningActions = DefaultListeningActions;
            }

            public void Handler(object sender, EventArgs args)
            {
                DeliverEvent(sender, args);
            }
        }

        internal WeakEventRouter() { }
    }
}
