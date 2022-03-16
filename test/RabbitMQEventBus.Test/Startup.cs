using Deveplex.EventBus.DependencyInjection;
using Deveplex.EventBus.RabbitMQ;
using Deveplex.EventBus.RabbitMQ.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RabbitMQEventBus.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
