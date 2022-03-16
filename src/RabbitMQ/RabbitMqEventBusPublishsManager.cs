/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Deveplex.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deveplex.EventBus.RabbitMQ
{
    public class RabbitMqEventBusPublishsManager : EventBusManager, IEventBusPublishsManager
    {
        public Task AddPublish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            throw new NotImplementedException();
        }
    }
}
