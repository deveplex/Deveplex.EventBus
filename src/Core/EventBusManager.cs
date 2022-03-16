/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Deveplex.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Deveplex.EventBus
{
    public class EventBusManager : IEventBusManager
    {
        public string GetEventName<TEvent>() where TEvent : IEvent
            => GetEventName(typeof(TEvent));

        public string GetEventName<TEvent>(TEvent @event) where TEvent : IEvent
            => GetEventName(@event.GetType());

        private string GetEventName(Type eventType)
        {
            //var genericType = typeof(IEvent);
            //if (genericType.GetTypeInfo().IsAssignableFrom(@event.GetType().GetTypeInfo()))
            //{
            //    return @event.EventName;
            //}

            return eventType.Name;
        }
    }
}
