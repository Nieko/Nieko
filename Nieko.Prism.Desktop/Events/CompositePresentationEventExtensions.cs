using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Prism.Events
{
    public static class CompositePresentationEventExtensions
    {
        public static void Publish<TMessageType>(this IEventAggregator aggregator, TMessageType message)
            where TMessageType : CompositePresentationEvent<TMessageType>, new()
        {
            aggregator.GetEvent<TMessageType>().Publish(message);
        }

        public static void Subscribe<TMessageType>(this IEventAggregator aggregator, Action<TMessageType> action)
            where TMessageType : CompositePresentationEvent<TMessageType>, new()
        {
            aggregator.GetEvent<TMessageType>().Subscribe(action);
        }

        public static void Unsubscribe<TMessageType>(this IEventAggregator aggregator, Action<TMessageType> action)
            where TMessageType : CompositePresentationEvent<TMessageType>, new()
        {
            aggregator.GetEvent<TMessageType>().Unsubscribe(action);  
        }

    }
}