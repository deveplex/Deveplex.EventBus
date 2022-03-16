/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Threading.Tasks;

namespace Deveplex.EventBus.Abstractions
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent @event)
            where TEvent : IEvent;

        Task Subscribe<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        Task SubscribeDynamic<THandler>(string eventName)
            where THandler : IDynamicIntegrationEventHandler;

        Task UnsubscribeDynamic<THandler>(string eventName)
            where THandler : IDynamicIntegrationEventHandler;

        Task Unsubscribe<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IIntegrationEventHandler<TEvent>;
    }
}
