/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Deveplex.EventBus
{
    public abstract class EventBusContextOptionsBuilder
    {
        private EventBusContextOptions _options;

        public EventBusContextOptionsBuilder()
            : this(new EventBusContextOptions<EventBusContext>())
        {
        }

        public EventBusContextOptionsBuilder([NotNull] EventBusContextOptions options)
        {
            //Check.NotNull(options, nameof(options));

            _options = options;
        }

        public virtual EventBusContextOptions Options
            => _options;

        public IServiceProvider ServiceProvider { get; private set; }

        public virtual EventBusContextOptionsBuilder UseApplicationServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            return this;
        }

        public virtual void AddOrUpdateExtension<TExtension>([NotNull] TExtension extension)
            where TExtension : class, IEventBusContextOptionsExtension
       {
            //Check.NotNull(extension, nameof(extension));

            _options = _options.WithExtension(extension);
        }

    //public abstract EventBusContextOptionsBuilder WithOption(Func<EventBusContextOptionsExtension, EventBusContextOptionsExtension> withFunc);

    //public abstract EventBusContextOptionsBuilder WithOption(Action<EventBusContextOptions> withFunc);

    //void AddOrUpdateOptions<TOptions>(TOptions options)
    //{
    //    Check.NotNull(extension, nameof(extension));

    //    _options = _options.WithExtension(extension);
    //}

}

public class EventBusContextOptionsBuilder<TContext> : EventBusContextOptionsBuilder where TContext : EventBusContext
{
    public EventBusContextOptionsBuilder() : this(new EventBusContextOptions<TContext>())
    {
    }

    public EventBusContextOptionsBuilder([NotNull] EventBusContextOptions<TContext> options)
        : base(options)
    {
    }

    public new virtual EventBusContextOptions<TContext> Options
        => (EventBusContextOptions<TContext>)base.Options;


    //public override EventBusContextOptionsBuilder WithOption(Func<EventBusContextOptionsExtension, EventBusContextOptionsExtension> withFunc)
    //{
    //   var ex = withFunc(Options.FindExtension<EventBusContextOptionsExtension>() ?? new EventBusContextOptionsExtension());

    //    return this;
    //}


    //public override EventBusContextOptionsBuilder WithOption(Action<EventBusContextOptions> withFunc)
    //{
    //    withFunc(Options ?? new EventBusContextOptions<TContext>());

    //    return this;
    //}
}
}
