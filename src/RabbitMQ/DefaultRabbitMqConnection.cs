/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace Deveplex.EventBus.RabbitMQ
{
    internal class DefaultRabbitMqConnection : IRabbitMqConnection
    {
        private RabbitMqConnectionOptions _options;
        private readonly ILogger _logger;

        private IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        object sync_root = new object();

        public DefaultRabbitMqConnection(IOptions<RabbitMqConnectionOptions> optionsAccessor, ILoggerFactory logger)
        {
            _options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _logger = logger.CreateLogger(GetType());

            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                DispatchConsumersAsync = true
            };

            if (_options.Port > 0)
            {
                factory.Port = _options.Port;
            }
            if (!string.IsNullOrEmpty(_options.UserName))
            {
                factory.UserName = _options.UserName;
            }
            if (!string.IsNullOrEmpty(_options.Password))
            {
                factory.Password = _options.Password;
            }
            if (!string.IsNullOrEmpty(_options.VirtualHost))
            {
                factory.VirtualHost = _options.VirtualHost;
            }
            if (_options.ConnectionTimeout > 0)
            {
                factory.ContinuationTimeout = TimeSpan.FromMilliseconds(_options.ConnectionTimeout);
            }
            _connectionFactory = factory;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public bool Connect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (sync_root)
            {
                _connection = _connectionFactory.CreateConnection();

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    return false;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            Connect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            Connect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            Connect();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                Disconnect();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
