/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus.Abstractions
{
    public interface IEventBusManager
    {
        string GetEventName<TEvent>() where TEvent : IEvent;

        string GetEventName<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
