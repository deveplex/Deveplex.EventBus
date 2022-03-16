using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deveplex.EventBus.RabbitMQ;
using Deveplex.EventBus;

namespace RabbitMQEventBus.Test.Pages
{
    public class IndexModel : PageModel
    {
        IServiceProvider _services;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IServiceProvider services, ILogger<IndexModel> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async void OnGet()
        {
            //var context = _services.GetService<RabbitMqEventBusContext>();
            var eventBus = _services.GetService<RabbitMqEventBus<RabbitMqEventBusContext>>();

            await eventBus.Publish(new EmailTokenEvent()
            {
                Destination = new string[] { "email" },
                subject = "subject",
                Body = "message"
            });
        }
    }
    public class EmailTokenEvent : IntegrationEvent
    {
        public EmailTokenEvent()
        {
            EventName = "GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG";
            Mode = 2;
            ExpiresIn = 60000;
        }

        public string[] Destination { get; set; }

        public string subject { get; set; }

        public string Body { get; set; }
    }
}
