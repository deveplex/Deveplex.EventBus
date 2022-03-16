/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Deveplex.EventBus.DependencyInjection
{
    public static class EventBusServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBusContext<TContext>(
            this IServiceCollection serviceCollection,
            Action<EventBusContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
            ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
            where TContext : EventBusContext
            => AddEventBusContext<TContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

        public static IServiceCollection AddEventBusContext<TContextService, TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<EventBusContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
            ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
            where TContextImplementation : EventBusContext, TContextService
            => AddEventBusContext<TContextService, TContextImplementation>(
                serviceCollection,
                optionsAction == null
                    ? (Action<IServiceProvider, EventBusContextOptionsBuilder>)null
                    : (p, b) => optionsAction(b), contextLifetime, optionsLifetime);


        public static IServiceCollection AddEventBusContext<TContext>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, EventBusContextOptionsBuilder> optionsAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
            ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
            where TContext : EventBusContext
            => AddEventBusContext<TContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

        public static IServiceCollection AddEventBusContext<TContextService, TContextImplementation>(
                [NotNull] this IServiceCollection serviceCollection,
                Action<IServiceProvider, EventBusContextOptionsBuilder> optionsAction,
                ServiceLifetime contextLifetime = ServiceLifetime.Singleton,
                ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
                where TContextImplementation : EventBusContext, TContextService
        {
            //Check.NotNull(serviceCollection, nameof(serviceCollection));

            if (contextLifetime == ServiceLifetime.Singleton)
            {
                optionsLifetime = ServiceLifetime.Singleton;
            }

            if (optionsAction != null)
            {
                CheckContextConstructors<TContextImplementation>();
            }

            AddCoreServices<TContextImplementation>(serviceCollection, optionsAction, optionsLifetime);

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime));

            return serviceCollection;
        }

        private static void AddCoreServices<TContextImplementation>(
            IServiceCollection serviceCollection,
            Action<IServiceProvider, EventBusContextOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime)
            where TContextImplementation : EventBusContext
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(EventBusContextOptions<TContextImplementation>),
                    p => CreateContextOptions<TContextImplementation>(p, optionsAction),
                    optionsLifetime));

            serviceCollection.Add(
                new ServiceDescriptor(
                    typeof(EventBusContextOptions),
                    p => p.GetRequiredService<EventBusContextOptions<TContextImplementation>>(),
                    optionsLifetime));
        }

        private static EventBusContextOptions<TContext> CreateContextOptions<TContext>(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, EventBusContextOptionsBuilder> optionsAction)
            where TContext : EventBusContext
        {
            var builder = new EventBusContextOptionsBuilder<TContext>(new EventBusContextOptions<TContext>());

            builder.UseApplicationServiceProvider(applicationServiceProvider);

            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }

        private static void CheckContextConstructors<TContext>()
            where TContext : EventBusContext
        {
            var declaredConstructors = typeof(TContext).GetTypeInfo().DeclaredConstructors.ToList();
            if (declaredConstructors.Count == 1
                && declaredConstructors[0].GetParameters().Length == 0)
            {
                throw new ArgumentException("CoreStrings.DbContextMissingConstructor(typeof(TContext).ShortDisplayName())");
            }
        }
    }

}
