## 事件总线封装类库
---
封装这个类库主要是想让事件总线在使用RabbitMQ的方式像使用EntityFramework Core的注入方式一样，欢迎大家提出意见和建议，改进功能，贡献代码。喜欢的请star。

### 使用方法
---
在配置文件中添加连接字符串

    "ConnectionStrings": {
        "RabbitmqConnection": "server=localhost;userid=guest;password=guest"
    }

在Startup中添加注入

    services.AddEventBusContext<RabbitMqEventBusContext>(o =>
    {
        o.UseRabbitMQ(Configuration.GetConnectionString("RabbitmqConnection"));
    });
    services.AddEventBus<RabbitMqEventBus<RabbitMqEventBusContext>>(o =>
    {
        o.WithOptions<RabbitMqOptions>(option =>
        {
            option.Durable = true;
            option.ExchangeName = "rabbitmq.test";
        });
    });

派生发布的事件类和事件处理类

    public class EmailTokenEvent : IntegrationEvent
    {
        public EmailTokenEvent()
        {
        }

        public string[] Destination { get; set; }

        public string subject { get; set; }

        public string Body { get; set; }
    }

    public class EmailTokenEventHandler : IIntegrationEventHandler<EmailTokenEvent>
    {
        public async Task HandleAsync(EmailTokenEvent @event)
        {
        }
    }

注册订阅处理

    var eventBus = app.ApplicationServices.GetRequiredService<RabbitMqEventBus<RabbitMqEventBusContext>>();
    eventBus.Subscribe<EmailTokenEvent, EmailTokenEventHandler>();


发送消息到RabbitMQ队列

    var eventBus = _services.GetRequiredService<RabbitMqEventBus<RabbitMqEventBusContext>>();
    await eventBus.Publish(new EmailTokenEvent()
    {
        Destination = new string[] { "email" },
        subject = "subject",
        Body = "message"
    });



