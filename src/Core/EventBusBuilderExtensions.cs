/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Deveplex.EventBus.Abstractions;
using Deveplex.EventBus.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus.DependencyInjection
{
    public static class EventBusBuilderExtensions
    {
        public static IServiceCollection AddEventBus<TEventBus>(this IServiceCollection services,
            Action<EventBusOptionsBuilder> optionsAction = null,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TEventBus : IEventBus
            => AddEventBus<TEventBus, TEventBus>(services, optionsAction, lifetime);

        public static IServiceCollection AddEventBus<TEventBusService, TEventBusImplementation>(
            this IServiceCollection services,
            Action<EventBusOptionsBuilder> optionsAction = null,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TEventBusImplementation : IEventBus
        {
            var bui = new EventBusOptionsBuilder(services);
            optionsAction?.Invoke(bui);

            //services.Configure()

            services.AddSingleton<IEventBusPublishsManager, InMemoryEventBusPublishsManager>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.TryAdd(new ServiceDescriptor(
                typeof(TEventBusService)
                , typeof(TEventBusImplementation)
                , lifetime));

            return services;
        }
    }
}
