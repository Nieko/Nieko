using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nieko.Infrastructure.EventAggregation;
using System.Linq.Expressions;
using System.Reflection;
using Nieko.Infrastructure;
using Nieko.Prism.Events;
using Nieko.Infrastructure.Reflection;
using Nieko.Infrastructure.Logging;

namespace Nieko.Modules.Architecture
{
    public class InfrastuctureEventAggregator : IInfrastructureEventAggregator
    {
        object _LockObject = new object();
        IEventAggregatorFacade _EventFacade;
        IDictionary<Type, object> _PublishActions;
        IDictionary<Type, object> _SubscribeActions;
        IDictionary<Type, Type> _EventTypeMappings;
        List<object> _WeakHandlers = new List<object>();
        List<KeyValuePair<WeakReference, Action>> _WeakUnsubscribers = new List<KeyValuePair<WeakReference, Action>>();

        protected IDictionary<Type, object> PublishActions
        {
            get
            {
                if (_PublishActions == null)
                {
                    PopulateActions();
                }

                return _PublishActions;
            }
        }

        protected IDictionary<Type, object> SubscribeActions
        {
            get
            {
                if (_SubscribeActions == null)
                {
                    PopulateActions();
                }

                return _SubscribeActions;
            }
        }

        protected IDictionary<Type, Type> EventTypeMappings
        {
            get
            {
                if (_EventTypeMappings == null)
                {
                    var interfaces = AssemblyHelper.FindTypes(t => t.IsInterface && typeof(IInfrastructureEvent).IsAssignableFrom(t) && typeof(IInfrastructureEvent) != t);
                    var implementations = interfaces
                        .Select(i => new 
                        { 
                            Interface = i, 
                            Implementations = AssemblyHelper.FindTypes(t => !(t.IsInterface || t.IsGenericTypeDefinition || t.IsAbstract) && i.IsAssignableFrom(t)) 
                        });

                    var mappingErrors = string.Empty;

                    if (implementations.Any(i => !i.Implementations.Any()))
                    {
                        mappingErrors += implementations.Where(i => !i.Implementations.Any())
                            .Aggregate(string.Empty, (current, i) => current += Environment.NewLine + "No concrete implementation of " + i.Interface.FullName);   
                    }

                    if (implementations.Any(i => i.Implementations.Count() > 1))
                    {
                        mappingErrors += implementations.Where(i => i.Implementations.Count() > 1)
                            .Aggregate(string.Empty, (current, i) => current += Environment.NewLine + "More than one concrete implementation of " + i.Interface.FullName + " found.");   
                    }

                    if (!string.IsNullOrEmpty(mappingErrors))
                    {
                        throw new InfrastructureEventAggregatorInitializationException(mappingErrors); 
                    }

                    _EventTypeMappings = implementations.ToDictionary(i => i.Interface, i => i.Implementations.First());
                }

                return _EventTypeMappings;
            }
        }

        public InfrastuctureEventAggregator(IEventAggregatorFacade eventFacade)
        {
            _EventFacade = eventFacade;
        }

        public TEvent CreateEvent<TEvent>()
            where TEvent : IInfrastructureEvent
        {
            TEvent infrastructureEvent;

            if (EventTypeMappings.ContainsKey(typeof(TEvent)))
            {
                infrastructureEvent = (TEvent)(Activator.CreateInstance(EventTypeMappings[typeof(TEvent)]));
            }
            else
            {
                infrastructureEvent = Activator.CreateInstance<TEvent>();
            }

            return infrastructureEvent;
        }

        public void Publish<TEvent>(TEvent args) where TEvent : IInfrastructureEvent
        {
            Logger.Instance.Log(typeof(TEvent).FullName + " Infrastructure event published");
            (PublishActions[typeof(TEvent)] as Action<TEvent>)(args);
        }

        public TEvent Publish<TEvent>() where TEvent : IInfrastructureEvent
        {
            var args = CreateEvent<TEvent>();
            Publish(args);

            return args;
        }

        public void Subscribe<TEvent>(Action<TEvent> onEvent) where TEvent : IInfrastructureEvent
        {
            Subscribe(onEvent, true);
        }

        public void Subscribe<TEvent>(Action<TEvent> onEvent, bool keepAlive) where TEvent : IInfrastructureEvent
        {
            Logger.Instance.Log(typeof(TEvent).FullName + " Infrastructure event subscribed to");
            (SubscribeActions[typeof(TEvent)] as Action<Action<TEvent>, bool>)(onEvent, keepAlive);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> onEvent) where TEvent : IInfrastructureEvent
        {
            Logger.Instance.Log(typeof(TEvent).FullName + " Infrastructure event unsubscribed from");
            lock(_LockObject)
            {
                if (_WeakUnsubscribers.Any(kvp => kvp.Key.IsAlive && ((Action<TEvent>)kvp.Key.Target) == onEvent))
                {
                    var unsubscription = _WeakUnsubscribers.First(kvp => kvp.Key.IsAlive && ((Action<TEvent>)kvp.Key.Target) == onEvent);
                    unsubscription.Value();
                    _WeakUnsubscribers.Remove(unsubscription);
                }
            }
        }

        private void PopulateActions()
        {
            _PublishActions = new Dictionary<Type, object>();
            _SubscribeActions = new Dictionary<Type, object>();
            MethodInfo facadeCall;
            ParameterExpression from;
            LambdaExpression facadeLambda;
            Type actionType;

            foreach (var mapping in EventTypeMappings)
            {
                facadeCall = typeof(IEventAggregatorFacade).GetMethod("Publish").MakeGenericMethod(mapping.Value);
                from = Expression.Parameter(mapping.Key, "from");

                facadeLambda = Expression.Lambda(
                    Expression.Call(
                        Expression.Constant(_EventFacade), 
                        facadeCall,
                        Expression.MakeUnary(
                            ExpressionType.Convert,
                            from,
                            mapping.Value)),
                    from);

                _PublishActions.Add(mapping.Key, facadeLambda.Compile());

                actionType = typeof(Action<>).MakeGenericType(mapping.Key);
                facadeCall = typeof(InfrastuctureEventAggregator).GetMethod("AddSubscribeAction", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(mapping.Key, mapping.Value);
                
                facadeLambda = Expression.Lambda(
                    Expression.Call(
                        Expression.Constant(this, typeof(InfrastuctureEventAggregator)),
                        facadeCall));

                facadeLambda.Compile().DynamicInvoke();  
            }
        }

        private void AddSubscribeAction<From, To>()
            where To : From
        {
            Action<Action<From>, bool> subscribe = (Action<From> from, bool keepAlive) =>
            {
                if (keepAlive)
                {
                    Func<Action<From>, Action<To>> actionMap = o => { return (to) => o((From)to); };
                    _EventFacade.Subscribe(actionMap(from), true);
                }
                else
                {
                    var weakFrom = new WeakReference(from);
                    Action<To> toAction = null;
                    toAction = o =>
                        {
                            if (weakFrom.IsAlive)
                            {
                                (weakFrom.Target as Action<From>)((From)o);
                            }
                            else
                            {
                                _WeakHandlers.Remove(toAction);
                            }
                        };
                    _WeakHandlers.Add(toAction);
                    _WeakUnsubscribers.Add(new KeyValuePair<WeakReference,Action>(weakFrom, () =>
                        {
                            _EventFacade.Unsubscribe(toAction);
                        }));
                    _EventFacade.Subscribe(toAction, false);
                }
            };

            _SubscribeActions.Add(typeof(From), subscribe); 
        }
        
    }
}
