/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Deveplex.EventBus
{
    public class EventBusContext : IDisposable

    {
        private readonly EventBusContextOptions _options;

        private EventBusFacade _instance;
        private bool _initializing;
        private bool _disposed;

        protected EventBusContext()
            : this(new EventBusContextOptions<EventBusContext>())
        {
        }

        public EventBusContext([NotNull] EventBusContextOptions options)
        {
            //Check.NotNull(options, nameof(options));

            if (!options.ContextType.IsAssignableFrom(GetType()))
            {
                throw new InvalidOperationException(string.Format("The EventBusContextOptions passed to the {0} constructor must be a EventBusContextOptions&lt;{1}&gt;.", GetType().Name));
            }

            _options = options;
        }

        public virtual EventBusFacade Instance
        {
            get
            {
                CheckDisposed();

                return _instance = _instance ?? new EventBusFacade(this);
            }
        }

        public virtual TExtension GetExtension<TExtension>()
            where TExtension : class, IEventBusContextOptionsExtension
        {
            CheckDisposed();

            var extension = _options.FindExtension<TExtension>();
            //if (extension == null)
            //{
            //    throw new InvalidOperationException(R.GetString(ResourceDescriber.OptionsExtensionNotFound, typeof(TExtension).ShortDisplayName()));
            //}
            return extension;
        }

        protected internal virtual void OnConfiguring(EventBusContextOptionsBuilder optionsBuilder)
        {
        }

        [DebuggerStepThrough]
        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed context instance.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool Dispose(bool disposed)
        {
            if (!_disposed)
            {
                Instance.Dispose();
                _disposed = disposed;
            }
            return _disposed;
        }
    }



    public class EventBusFacade : IDisposable
    {
        private readonly EventBusContext _context;

        private bool _disposed;

        public EventBusFacade([NotNull] EventBusContext context)
        {
            //Check.NotNull(context, nameof(context));

            _context = context;

            //var options = context.
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool Dispose(bool disposed)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
            return _disposed;
        }
    }
}
