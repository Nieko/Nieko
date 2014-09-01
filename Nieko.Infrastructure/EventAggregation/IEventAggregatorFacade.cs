using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nieko.Infrastructure.EventAggregation
{
    /// <summary>
    /// Provide simple access to an Event Aggregator implementation so that
    /// it may be called from <see cref="IInfrastructureEventAggregator"/>.
    /// <remarks>
    /// In conjunction with <see cref="IInfrastructureEventAggregator"/> acts as a bridge between
    /// the abstract Infrastructure Event system and whatever the concrete implementation is.
    /// Refer <see cref="IInfrastructureEventAggregator"/> for details on the methods
    /// </remarks>
    /// </summary>
    public interface IEventAggregatorFacade
    {
        void Publish<TEvent>(TEvent args);
        void Subscribe<TEvent>(Action<TEvent> onEvent);
        void Subscribe<TEvent>(Action<TEvent> onEvent, bool keepAlive);
        void Unsubscribe<TEvent>(Action<TEvent> onEvent);
    }
}
