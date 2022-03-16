/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus.RabbitMQ
{
    public class RabbitMqEventBusContext : EventBusContext
    {
        public RabbitMqEventBusContext(EventBusContextOptions options)
            : base(options)
        {

        }
    }
}
