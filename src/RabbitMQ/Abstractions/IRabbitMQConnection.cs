/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using RabbitMQ.Client;
using System;

namespace Deveplex.EventBus.RabbitMQ
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }

        bool Connect();

        IModel CreateModel();
    }
}
