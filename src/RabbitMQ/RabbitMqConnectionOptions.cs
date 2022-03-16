/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Deveplex.EventBus.RabbitMQ
{
    internal class RabbitMqConnectionOptions : IEventBusContextOptionsExtension
    {
        /// <summary>
        /// The host to connect to.
        /// If you want connect to the cluster, you can assign like “192.168.1.111,192.168.1.112”
        /// </summary>
        public string HostName { get; set; } = "localhost";

        /// <summary>
        /// Username to use when authenticating to the server.
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// Password to use when authenticating to the server.
        /// </summary>
        public string Password { get; set; } = "guest";

        /// <summary>
        /// Virtual host to access during this connection.
        /// </summary>
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// The port to connect on.
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// Milliseconds
        /// </summary>
        public int ConnectionTimeout { get; set; } = 30000;

        /// <summary>
        /// The number of connection retries, the retry will stop when the threshold is reached.
        /// Default is 3 times.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        public virtual RabbitMqConnectionOptions WithConnectionString(string connectionString)
        {
            //Check.NotEmpty(connectionString, nameof(connectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var connString = new RabbitMqConnectionString(connectionString);

            //HostName:Port\VirtualHost
            var server = connString[RabbitMqConnectionString.KEY.Server];
            if (!string.IsNullOrEmpty(server))
            {
                var addr = server.Split('\\', '/');
                if (addr.Length > 1)
                {
                    VirtualHost = addr[1];
                }
                var host = addr[0].Split(':');
                if (host.Length > 1)
                {
                    Port = int.Parse(addr[1]);
                }
                HostName = host[0];
            }

            var userid = connString[RabbitMqConnectionString.KEY.User_ID];
            if (!string.IsNullOrEmpty(userid))
            {
                UserName = userid;
            }

            var pwd = connString[RabbitMqConnectionString.KEY.Password];
            if (!string.IsNullOrEmpty(pwd))
            {
                Password = pwd;
            }

            var timeout = connString[RabbitMqConnectionString.KEY.Timeout];
            if (!string.IsNullOrEmpty(timeout))
            {
                if (int.TryParse(timeout, out var v))
                {
                    ConnectionTimeout = v;
                }
            }

            var retrycount = connString[RabbitMqConnectionString.KEY.Connect_Retry_Count];
            if (!string.IsNullOrEmpty(retrycount))
            {
                if (int.TryParse(retrycount, out var v))
                {
                    RetryCount = v;
                }
            }
            return this;
        }
    }
}
