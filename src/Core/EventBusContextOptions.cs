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
    public abstract class EventBusContextOptions
    {
        private readonly IReadOnlyDictionary<Type, IEventBusContextOptionsExtension> _extensions;

        protected EventBusContextOptions(
            [NotNull] IReadOnlyDictionary<Type, IEventBusContextOptionsExtension> extensions)
        {
            //Check.NotNull(extensions, nameof(extensions));

            _extensions = extensions;
        }

        public virtual IEnumerable<IEventBusContextOptionsExtension> Extensions
            => _extensions.Values;

        public virtual TExtension FindExtension<TExtension>()
            where TExtension : class, IEventBusContextOptionsExtension
            => _extensions.TryGetValue(typeof(TExtension), out var extension) ? (TExtension)extension : null;

        public abstract EventBusContextOptions WithExtension<TExtension>([NotNull] TExtension extension)
            where TExtension : class, IEventBusContextOptionsExtension;

        public abstract Type ContextType { get; }
    }

    public class EventBusContextOptions<TContext> : EventBusContextOptions
    {
        public EventBusContextOptions()
            : base(new Dictionary<Type, IEventBusContextOptionsExtension>())
        {

        }

        public EventBusContextOptions(
            [NotNull] IReadOnlyDictionary<Type, IEventBusContextOptionsExtension> extensions)
            : base(extensions)
        {
        }

        public override EventBusContextOptions WithExtension<TExtension>([NotNull] TExtension extension)
        {
            //Check.NotNull(extension, nameof(extension));

            var extensions = Extensions.ToDictionary(p => p.GetType(), p => p);
            extensions[typeof(TExtension)] = extension;

            return new EventBusContextOptions<TContext>(extensions);
        }

        public override Type ContextType
            => typeof(TContext);
    }
}
