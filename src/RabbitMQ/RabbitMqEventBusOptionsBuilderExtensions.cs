/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Deveplex.EventBus.RabbitMQ.DependencyInjection
{

    public static class EventBusContextOptionsBuilderExtensions
    {
        public static EventBusContextOptionsBuilder UseRabbitMQ(this EventBusContextOptionsBuilder builder,
            [NotNull] string connectionString,
            Action<RabbitMqContextOptionsBuilder> optionsAction = null)
        {
            //Check.NotNull(builder, nameof(builder));
            //Check.NotEmpty(connectionString, nameof(connectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            // "server=HostName:Port\VirtualHost;uid=guest;pwd=guest",
            var extension = (RabbitMqConnectionOptions)GetOrCreateExtension(builder).WithConnectionString(connectionString);
            builder.AddOrUpdateExtension(extension);

            return builder;
        }

        //public static EventBusContextOptionsBuilder UseRabbitMq(this EventBusContextOptionsBuilder builder,

        //    Action<RabbitMqContextOptionsBuilder> optionsAction = null)
        //{
        //    Check.NotNull(builder, nameof(builder));

        //    var extension = null;
        //    builder.WithOption(e => e.WithExtension(extension));

        //    //ConfigureWarnings(optionsBuilder);

        //    //configureOptions?.Invoke(new RabbitMQContextOptionsBuilder(optionsBuilder));

        //    return builder;
        //}


        private static RabbitMqConnectionOptions GetOrCreateExtension(EventBusContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<RabbitMqConnectionOptions>()
                ?? new RabbitMqConnectionOptions();

    }

    public static class EventBusOptionsBuilderExtensions
    {
        public static EventBusOptionsBuilder WithRabbitMqOptions(this EventBusOptionsBuilder builder, Action<RabbitMqOptions> configureOptions)
        {
            builder.WithOptions(configureOptions);
            return builder;
        }
    }
}

namespace Deveplex.EventBus.RabbitMQ
{
    public class RabbitMqContextOptionsBuilder
    {
        EventBusContextOptions _options;

        internal RabbitMqContextOptionsBuilder([NotNull] EventBusContextOptions options)
        {
            //Check.NotNull(options, nameof(options));

            _options = options;
        }

        //protected virtual TBuilder WithOption([NotNull] Func<TExtension, TExtension> setAction)
        //{
        //    ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(
        //        setAction(OptionsBuilder.Options.FindExtension<TExtension>() ?? new TExtension()));

        //    return (TBuilder)this;
        //}
    }
}
