using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using l = System.Linq.Expressions;

namespace Nieko.Infrastructure.ComponentModel
{
    internal class WeakEventRouter<TManager, TPublisher, TArgs, TSubscriber> : IWeakEventRouter
        where TManager : WeakEventManagerBase<TManager, TPublisher>, new()
        where TPublisher : class
        where TArgs : EventArgs
    {
        private WeakReference _Subscriber;
        private TPublisher _Publisher;
        private Action<TSubscriber, TPublisher, TArgs> _HandlerMethod;

        internal static T CreateInstance<T>(TSubscriber subscriber, TPublisher publisher, Action<TSubscriber, TPublisher, TArgs> handlerMethod)
            where T : WeakEventRouter<TManager, TPublisher, TArgs, TSubscriber>, new()
        {
            var instance = new T();

            instance._Subscriber = new WeakReference(subscriber);
            instance._Publisher = publisher;
            instance._HandlerMethod = handlerMethod;

            WeakEventManagerBase<TManager, TPublisher>.AddListener(instance._Publisher, instance);

            return instance;

        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (_Subscriber == null)
            {
                return true;
            }

            if (!_Subscriber.IsAlive)
            {
                CancelSubscription();
                return true;
            }

            if (typeof(TManager) != managerType
                || !typeof(TPublisher).IsAssignableFrom(sender.GetType())
                || !typeof(TArgs).IsAssignableFrom(e.GetType()))
            {
                return true;
            }

            _HandlerMethod((TSubscriber)_Subscriber.Target, _Publisher, (TArgs)e);

            return true;
        }

        public void CancelSubscription()
        {
            WeakEventManagerBase<TManager, TPublisher>.RemoveListener(_Publisher, this);
            _Subscriber = null;
            _Publisher = null;
        }
    }
}
