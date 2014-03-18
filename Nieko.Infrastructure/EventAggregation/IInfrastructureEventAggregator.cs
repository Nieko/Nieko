using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Event Aggregation facade. Allows event aggregation to be used within
    /// the framework without knowledge or strong references to whatever is actually  
    /// implementing the weak event aggregation. Also simplifies event aggregation 
    /// to single, parameterised methods.
    /// </summary>
    /// <remarks>
    /// NOTE : [TEvent] parameters expect individual event interface types NOT the 
    /// concrete type implementing the event
    /// </remarks>
    public interface IInfrastructureEventAggregator
    {
        /// <summary>
        /// For use when an event needs to be customised (i.e. by settings properties
        /// holding argument information) before publishing. If an event does not
        /// need pre-publishing handling, use Publish[TEvent]() instead.
        /// </summary>
        /// <typeparam name="TEvent">Event interface</typeparam>
        /// <returns>Event implementation created</returns>
        TEvent CreateEvent<TEvent>()
            where TEvent : IInfrastructureEvent;
        /// <summary>
        /// Creates an event implementing <typeparamref name="TEvent"/> and 
        /// publishes it
        /// </summary>
        /// <typeparam name="TEvent">Event interface</typeparam>
        /// <returns>Event instance after subscribers have finished processing</returns>
        TEvent Publish<TEvent>()
            where TEvent : IInfrastructureEvent;
        /// <summary>
        /// Publishes a previously created event. If an event does not
        /// need pre-publishing handling, use Publish[TEvent]() instead.
        /// </summary>
        /// <typeparam name="TEvent">Event interface</typeparam>
        /// <param name="args">Event instance</param>
        void Publish<TEvent>(TEvent args)
            where TEvent : IInfrastructureEvent;
        /// <summary>
        /// Subscribes to event interface <typeparamref name="TEvent"/> and uses
        /// <paramref name="onEvent"/> to process it. Subscription is only continued
        /// as long as <paramref name="onEvent"/> is alive
        /// </summary>
        /// <typeparam name="TEvent">Event interface</typeparam>
        /// <param name="onEvent">Processing action</param>
        void Subscribe<TEvent>(Action<TEvent> onEvent)
            where TEvent : IInfrastructureEvent;
        /// <summary>
        /// Subscribes to event interface <typeparamref name="TEvent"/> and uses
        /// <paramref name="onEvent"/> to process it. If <paramref name="keepAlive"/>, subscription 
        /// is continued indefinitely by keeping a strong reference to <paramref name="onEvent"/>
        /// </summary>
        /// <typeparam name="TEvent">Event interface</typeparam>
        /// <param name="onEvent">Processing action</param>
        /// <param name="keepAlive">Keep processing action alive</param>
        void Subscribe<TEvent>(Action<TEvent> onEvent, bool keepAlive)
            where TEvent : IInfrastructureEvent;
        /// <summary>
        /// Manually unsubscribes from an event. Especially required for 
        /// subscriptions kept alive via Subscribe(processing, true). 
        /// </summary>
        /// <remarks>
        /// <paramref name="onEvent"/> must be the same as that passed 
        /// to Subscribe and it is recommended that a instance reference is kept
        /// to the Action[TEvent] if this method is to be called
        /// </remarks>
        /// <typeparam name="TEvent">Event interface</typeparam>
        /// <param name="onEvent">Processing action</param>
        void Unsubscribe<TEvent>(Action<TEvent> onEvent)
            where TEvent : IInfrastructureEvent;
    }
}
