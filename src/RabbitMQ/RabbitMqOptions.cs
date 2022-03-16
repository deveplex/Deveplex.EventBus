/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Deveplex.EventBus.RabbitMQ
{
    public class RabbitMqOptions
    {

        /// <summary> The direct exchange type. </summary>
        public string ExchangeType { get; set; } = "direct";

        /// <summary>
        /// Topic exchange name when declare a direct exchange.
        /// </summary>
        public string ExchangeName { get; set; } = "direct.default.router";

        /// <summary>
        /// 
        /// </summary>
        public string QueueName { get; set; } = "direct.default.queue";

        /// <summary>
        /// false：非持久化，true：持久化
        /// </summary>
        public bool Durable { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool AutoAck { get; set; } = false;

        /// <summary>
        /// Gets or sets queue message automatic deletion time (in milliseconds). Default no delete (0 is no delete).
        /// </summary>
        public int ExpiresIn { get; set; } = 0;

        /// <summary>
        /// The number of send retries, the retry will stop when the threshold is reached.
        /// Default is 5 times.
        /// </summary>
        public int RetryCount { get; set; } = 1;

        /// <summary>
        /// RabbitMQ native connection factory options
        /// </summary>
        //public Action<ConnectionFactory> ConnectionFactoryOptions { get; set; }
    }
}
