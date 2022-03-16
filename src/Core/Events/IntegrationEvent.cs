/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Deveplex.EventBus.Abstractions;
using System;

namespace Deveplex.EventBus
{
    public class IntegrationEvent : IEvent
    {
        public IntegrationEvent(string eventName) : this()
           => EventName = eventName;

        public IntegrationEvent(string id, string eventName) : this()
        {
            Id = id;
            EventName = eventName;
        }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid().ToString("n");
            EventName = GetType().Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string EventName { get; protected set; }

        /// <summary>
        /// 1：非持久化，2：持久化
        /// </summary>
        public byte Mode { get; set; } = (byte)DeliveryMode.NonPersistent;

        /// <summary>
        /// Gets or sets queue message automatic deletion time (in milliseconds). Default 0 ms (0 days).
        /// </summary>
        public int ExpiresIn { get; set; } = 0; //864000000;
    }
}
