/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Deveplex.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Deveplex.EventBus
{
    public abstract class EventBus<TContext> : IEventBus, IDisposable
        where TContext : class
    {
        private readonly TContext _context;
        //private readonly IEventBusPublishsManager _publishsManager;
        //private readonly IEventBusSubscriptionsManager _subscriptionsManager;
        private readonly ILogger _logger;

        public EventBus(TContext context,
            IEventBusPublishsManager publishsManager,
            IEventBusSubscriptionsManager subscriptionsManager,
            IServiceProvider services,
            ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger(GetType());
        }

        public abstract Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;

        public abstract Task Subscribe<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        public abstract Task SubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler;

        public abstract Task Unsubscribe<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        public abstract Task UnsubscribeDynamic<THandler>(string eventName) where THandler : IDynamicIntegrationEventHandler;

        public abstract void Dispose();
    }
}
