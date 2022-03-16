/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deveplex.EventBus.Abstractions
{
    public interface IEventBusPublishsManager : IEventBusManager
    {
        Task AddPublish<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }

    public interface IEventBusSubscriptionsManager : IEventBusManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddDynamicSubscription<THandler>(string eventName)
           where THandler : IDynamicIntegrationEventHandler;

        void AddSubscription<TEvent, THandler>()
           where TEvent : IEvent
           where THandler : IIntegrationEventHandler<TEvent>;

        void RemoveSubscription<TEvent, THandler>()
             where TEvent : IEvent
             where THandler : IIntegrationEventHandler<TEvent>;

        void RemoveDynamicSubscription<THandler>(string eventName)
            where THandler : IDynamicIntegrationEventHandler;

        bool HasSubscriptionsForEvent<TEvent>() where TEvent : IEvent;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>() where TEvent : IEvent;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        void Clear();
    }
}