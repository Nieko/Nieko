using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using l = System.Linq.Expressions;

namespace Nieko.Infrastructure.ComponentModel
{
    /// <summary>
    /// Facade for creation of type safe Weak Event Router, with the event subscription
    /// keeping neither publisher or subscriber alive. 
    /// </summary>
    /// <remarks>
    /// Requires the minimum parameters necessary to create a weak event subscription
    /// between a publisher and subscriber. The methods on this class do not require
    /// additional classes to be implemented or further calls to a Manager class.
    /// 
    /// Note that the actions performed must be unique per combination of publisher, 
    /// event argument and subscriber types. Attempts to provide different
    /// startListening / stopListening actions for a publisher-event argument-subscriber
    /// will be ignored
    /// </remarks>
    public static class WeakEventRouter
    {
        /// <summary>
        /// Weakly subscribes <paramref name="subscriber"/> to events on <paramref name="publisher"/>
        /// </summary>
        /// <typeparam name="TPublisher">Event(s) publisher</typeparam>
        /// <typeparam name="TArgs">Event arguments</typeparam>
        /// <typeparam name="TSubscriber">Subscriber to the event(s)</typeparam>
        /// <param name="subscriber">Event(s) publisher</param>
        /// <param name="publisher">Subscriber to the event(s)</param>
        /// <param name="startListening">Action to add handlers to <typeparamref name="TPublisher"/> events</param>
        /// <param name="stopListening">Action to remove handlers to <typeparamref name="TPublisher"/> events</param>
        /// <param name="handlerMethod">Event handler</param>
        /// <returns>Event Router</returns>
        private static IWeakEventRouter CreateInstance<TPublisher, TArgs, TSubscriber>(
            TSubscriber subscriber,
            TPublisher publisher,
            Action<TPublisher, IWeakEventDestination> startListening,
            Action<TPublisher, IWeakEventDestination> stopListening,
            Action<TSubscriber, TPublisher, TArgs> handlerMethod)
            where TPublisher : class
            where TArgs : EventArgs
        {
            // Check Listening actions for this combination of publisher, event argument and subscriber types have been
            // initialized.  
            lock (WeakEventRouter<TPublisher, TArgs, TSubscriber>.DefaultManager.ListeningActions)
            {
                if (WeakEventRouter<TPublisher, TArgs, TSubscriber>.DefaultManager.ListeningActions == WeakEventRouter<TPublisher, TArgs, TSubscriber>.DefaultListeningActions)
                {
                    var actions = new WeakEventRouter<TPublisher, TArgs, TSubscriber>.ListeningActions();

                    actions.StartListeningToAction = startListening;
                    actions.StopListeningToAction = stopListening;

                    WeakEventRouter<TPublisher, TArgs, TSubscriber>.DefaultManager.ListeningActions = actions;
                }
            }

            // Use method type parameters to build a strongly typed Weak Event Router and Manager
            return WeakEventRouter<WeakEventRouter<TPublisher, TArgs, TSubscriber>.DefaultManager,
                TPublisher,
                TArgs,
                TSubscriber>.CreateInstance<WeakEventRouter<WeakEventRouter<TPublisher, TArgs, TSubscriber>.DefaultManager,
                    TPublisher,
                    TArgs,
                    TSubscriber>>(subscriber, publisher, handlerMethod);
        }

        /// <summary>
        /// Weakly subscribes <paramref name="subscriber"/> to events on <paramref name="publisher"/>
        /// </summary>
        /// <typeparam name="TPublisher">Event(s) publisher</typeparam>
        /// <typeparam name="TArgs">Event arguments</typeparam>
        /// <typeparam name="TSubscriber">Subscriber to the event(s)</typeparam>
        /// <param name="subscriber">Event(s) publisher</param>
        /// <param name="publisher">Subscriber to the event(s)</param>
        /// <param name="argsProtoType">Function from which to infer <typeparamref name="TArgs"/>. i.e. <code>() => default(EventArgs)</code></param>
        /// <param name="startListening">Action to add handlers to <typeparamref name="TPublisher"/> events</param>
        /// <param name="stopListening">Action to remove handlers to <typeparamref name="TPublisher"/> events</param>
        /// <param name="handlerMethod">Event handler</param>
        /// <returns>Event Router</returns>
        public static IWeakEventRouter CreateInstance<TPublisher, TArgs, TSubscriber>(
            TSubscriber subscriber,
            TPublisher publisher,
            Func<TArgs> argsProtoType,
            Action<TPublisher, IWeakEventDestination> startListening,
            Action<TPublisher, IWeakEventDestination> stopListening,
            Action<TSubscriber, TPublisher, TArgs> handlerMethod)
            where TPublisher : class
            where TArgs : EventArgs
        {
            return CreateInstance(subscriber, publisher, startListening, stopListening, handlerMethod);
        }
    }
}
