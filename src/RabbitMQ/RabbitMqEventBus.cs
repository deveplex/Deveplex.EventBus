/* ************************************************************************
 * Copyright deveplex.com All rights reserved.
 * ***********************************************************************/

using Deveplex.EventBus.Abstractions;
using Deveplex.EventBus.Extensions;
using Deveplex.EventBus.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Deveplex.EventBus.RabbitMQ
{
    public class RabbitMqEventBus<TContext> : EventBus<TContext>, IDisposable
        where TContext : EventBusContext
    {
        private readonly TContext _context;
        private readonly IRabbitMqConnection _connection;
        private readonly IEventBusPublishsManager _publishsManager;
        private readonly IEventBusSubscriptionsManager _subscriptionsManager;
        private readonly RabbitMqOptions _options;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        private string _exchangeName;
        private string _exchangeType;
        private string _queueName;
        private IModel _consumerChannel;

        public RabbitMqEventBus(TContext context,
            IOptions<RabbitMqOptions> optionsAccessor,
            IEventBusPublishsManager publishsManager,
            IEventBusSubscriptionsManager subscriptionsManager,
            IServiceProvider services,
            ILoggerFactory logger)
            : base(context, publishsManager, subscriptionsManager, services, logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = optionsAccessor.Value ?? new RabbitMqOptions();
            _services = services;
            _logger = logger.CreateLogger(GetType());

            var extension = context.GetExtension<RabbitMqConnectionOptions>() ?? new RabbitMqConnectionOptions();
            _connection = new DefaultRabbitMqConnection(Options.Create(extension), logger);

            _publishsManager = publishsManager ?? new InMemoryEventBusPublishsManager();

            _subscriptionsManager = subscriptionsManager ?? new InMemoryEventBusSubscriptionsManager();
            _subscriptionsManager.OnEventRemoved += OnSubscriptionEventRemoved;

            _exchangeName = _options.ExchangeName ?? "";
            _exchangeType = _options.ExchangeType ?? "direct";
            _queueName = _options.QueueName ?? "default";

            _consumerChannel = CreateConsumerChannel();
        }

        public TContext Context => _context;

        public override Task Publish<TEvent>(TEvent @event)
        {
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_options.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            using (var channel = CreateChannel())
            {
                var eventName = _publishsManager.GetEventName(@event);// @event.GetType().Name;
                BindQueue(channel, eventName);

                _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = @event.Mode;
                    if (@event.ExpiresIn > 0)
                        properties.Expiration = @event.ExpiresIn.ToString();// 设置TTL，消息数据的过期时间

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    channel.BasicPublish(
                        exchange: _exchangeName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }

            return Task.CompletedTask;
        }

        public override Task Subscribe<TEvent, THandler>()
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(THandler).GetGenericTypeName());

            _subscriptionsManager.AddSubscription<TEvent, THandler>();
            StartBasicConsume();

            return Task.CompletedTask;
        }

        public override Task Unsubscribe<TEvent, THandler>()
        {
            var eventName = _subscriptionsManager.GetEventName<TEvent>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subscriptionsManager.RemoveSubscription<TEvent, THandler>();

            return Task.CompletedTask;
        }

        public override Task SubscribeDynamic<THandler>(string eventName)
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(THandler).GetGenericTypeName());

            DoInternalSubscription(eventName);
            _subscriptionsManager.AddDynamicSubscription<THandler>(eventName);
            StartBasicConsume();

            return Task.CompletedTask;
        }

        public override Task UnsubscribeDynamic<THandler>(string eventName)
        {
            _subscriptionsManager.RemoveDynamicSubscription<THandler>(eventName);

            return Task.CompletedTask;
        }

        private IModel CreateChannel()
        {
            var channel = CreateModel();

            channel.ExchangeDeclare(exchange: _exchangeName, type: _exchangeType);

            IDictionary<string, object> args = null;
            if (_options.ExpiresIn > 0)
            {
                if (args == null) args = new Dictionary<string, object>();
                //{ "x-message-ttl", 1800000},//队列上消息过期时间，应小于队列过期时间 60000 1800000
                //{ "x-dead-letter-exchange", "exchange-D"},//过期消息转向路由
                //{ "x-dead-letter-routing-key", routingKeyDead}//过期消息转向路由相匹配routingkey
                args.Add("x-message-ttl", _options.ExpiresIn);// 设置TTL，消息队列的过期时间
            }

            channel.QueueDeclare(
                _queueName,
                _options.Durable,   //队列是否持久化。false：队列在内存中，服务器挂掉后，队列就没了；true：服务器重启后，队列将会重新生成。注意：只是队列持久化，不代表队列中的消息持久化。
                false,  //队列是否专属,专属的范围针对的是连接,也就是说,一个连接下面的多个信道是可见的.对于其他连接是不可见的.连接断开后,该队列会被删除.注意,不是信道断开,是连接断开.并且,就算设置成了持久化,也会删除.
                _options.AutoDelete,    //如果所有消费者都断开连接了,是否自动删除.如果还没有消费者从该队列获取过消息或者监听该队列,那么该队列不会删除.只有在有消费者从该队列获取过消息后,该队列才有可能自动删除(当所有消费者都断开连接,不管消息是否获取完)
                args);

            return channel;
        }

        private IModel CreateModel()
        {
            if (!_connection.IsConnected)
            {
                var policy = RetryPolicy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(_options.RetryCount,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                        }
                    );

                policy.Execute(() =>
                {
                    _connection.Connect();
                });
            }

            return _connection.CreateModel();
        }

        private string GetRoutingKey(string eventName)
        {
            switch (_exchangeType.ToLower())
            {
                case "topic":
                    eventName = eventName + ".*";
                    break;
                case "fanout ":
                    eventName = "";
                    break;
                default: //direct
                    break;
            }
            return eventName;
        }

        private void BindQueue(IModel channel, string eventName)
        {
            channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: eventName);
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subscriptionsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                using (var channel = CreateChannel())
                {
                    BindQueue(channel, eventName);
                }
            }
        }

        private IModel CreateConsumerChannel()
        {
            var channel = CreateChannel();

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += ConsumerOnReceived;

                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: _options.AutoAck,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private void OnSubscriptionEventRemoved(object sender, string eventName)
        {
            using (var channel = CreateModel())
            {
                channel.QueueUnbind(
                    queue: _queueName,
                    exchange: _exchangeName,
                    routingKey: eventName);

                if (_subscriptionsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    _consumerChannel.Close();
                }
            }
        }

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs args)
        {
            var eventName = args.RoutingKey;
            var message = Encoding.UTF8.GetString(args.Body);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            if (!_options.AutoAck)
            {
                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html

                //deliveryTag： 传递标签,ulong 类型.它的范围隶属于每个信道，因此必须在收到消息的相同信道上确认，不同的信道将导致“未知的传递标签”协议异常并关闭通道
                //multiple： 确认一条消息还是多条。false 表示只确认e.DelivertTag这条消息；true表示确认小于等于e.DelivertTag的所有消息 
                _consumerChannel.BasicAck(args.DeliveryTag, multiple: false);
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (_subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _services.CreateScope())
                {
                    var subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler = scope.ServiceProvider.GetService<IDynamicIntegrationEventHandler>();
                            if (handler == null) continue;
                            dynamic eventData = JObject.Parse(message);

                            await Task.Yield();
                            await handler.HandleAsync(eventData);
                        }
                        else
                        {
                            var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                            if (handler == null) continue;
                            var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }

        public override void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }

            _subscriptionsManager.Clear();
        }
    }
}
