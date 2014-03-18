using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.EventAggregation;
using Nieko.Infrastructure;
using System.Linq.Expressions;
using Microsoft.Practices.Prism.Events;

namespace Nieko.Modules.Architecture
{
    public class PrismEventAggregator : IEventAggregatorFacade
    {
        object _LockObject = new object();
        IEventAggregator _EventAggregator;
        Dictionary<Type, Action<object>> _PublishMethods = new Dictionary<Type, Action<object>>();
        Dictionary<Type, Action<object>> _SubscribeMethods = new Dictionary<Type, Action<object>>();
        Dictionary<Type, Action<object>> _UnsubscribeMethods = new Dictionary<Type, Action<object>>();
        List<object> _ActionReferences = new List<object>();

        public PrismEventAggregator(IEventAggregator eventAggregator)
        {
            _EventAggregator = eventAggregator;
        }

        public void Publish<TMessageType>(TMessageType message) 
        {
            Action<object> method;

            lock (_LockObject)
            {
                if (!_PublishMethods.TryGetValue(typeof(TMessageType), out method))
                {
                    var methodInfo = typeof(CompositePresentationEventExtensions).GetMethod("Publish")
                    .MakeGenericMethod(typeof(TMessageType));

                    var ea = Expression.Parameter(typeof(IEventAggregator), "ea");
                    var m = Expression.Parameter(typeof(TMessageType), "m");

                    var call = Expression.Lambda<Action<IEventAggregator, TMessageType>>(
                        Expression.Call(
                            methodInfo,
                            ea,
                            m),
                        ea,
                        m).Compile();

                    method = o => call(_EventAggregator, (TMessageType)o);

                    _PublishMethods.Add(typeof(TMessageType), method);
                }
            }

            method(message);
        }

        public void Subscribe<TMessageType>(Action<TMessageType> action)
        {
            Subscribe(action, false); 
        }

        public void Subscribe<TMessageType>(Action<TMessageType> action, bool keepAlive)
        {
            Action<object> method;

            if (keepAlive)
            {
                _ActionReferences.Add(action);
            }

            lock (_LockObject)
            {
                if (!_SubscribeMethods.TryGetValue(typeof(TMessageType), out method))
                {
                    var methodInfo = typeof(CompositePresentationEventExtensions).GetMethod("Subscribe")
                    .MakeGenericMethod(typeof(TMessageType));

                    var ea = Expression.Parameter(typeof(IEventAggregator), "ea");
                    var m = Expression.Parameter(typeof(Action<TMessageType>), "m");
                    var aie = Expression.Parameter(typeof(TMessageType), "aie");

                    var call = Expression.Lambda<Action<IEventAggregator, Action<TMessageType>>>(
                        Expression.Call(
                            methodInfo,
                            ea,
                            m),
                        ea,
                        m).Compile();

                    method = (o) => call(_EventAggregator, o as Action<TMessageType>);

                    _SubscribeMethods.Add(typeof(TMessageType), method);

                }
            }

            method(action);
        }

        public void Unsubscribe<TMessageType>(Action<TMessageType> action)
        {
            Action<object> method;

            if (_ActionReferences.Contains(action))
            {
                _ActionReferences.Remove(action);
            }

            lock (_LockObject)
            {
                if (!_UnsubscribeMethods.TryGetValue(typeof(TMessageType), out method))
                {
                    var methodInfo = typeof(CompositePresentationEventExtensions).GetMethod("Unsubscribe")
                    .MakeGenericMethod(typeof(TMessageType));

                    var ea = Expression.Parameter(typeof(IEventAggregator), "ea");
                    var m = Expression.Parameter(typeof(Action<TMessageType>), "m");
                    var aie = Expression.Parameter(typeof(TMessageType), "aie");

                    var call = Expression.Lambda<Action<IEventAggregator, Action<TMessageType>>>(
                        Expression.Call(
                            methodInfo,
                            ea,
                            m),
                        ea,
                        m).Compile();

                    method = (o) => call(_EventAggregator, o as Action<TMessageType>);

                    _UnsubscribeMethods.Add(typeof(TMessageType), method);

                }
            }

            method(action);
        }
    }
}
