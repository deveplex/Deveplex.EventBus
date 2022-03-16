/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus
{
    public class EventBusOptionsBuilder
    {
        public EventBusOptionsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public virtual Action<TOptions> WithOptions<TOptions>(Action<TOptions> withFunc)
            where TOptions : class, new()
        {
            Services.Configure<TOptions>(withFunc);
            return withFunc;
        }

    }
}
