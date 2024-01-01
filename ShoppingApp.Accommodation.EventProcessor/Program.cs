using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ShoppingApp.Events;
using ShoppingApp.Events.ServiceBus;
using ShoppingApp.Accommodation.EventProcessor;

namespace Example.Accommodation.EventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddServiceBusClient(Environment.GetEnvironmentVariable("SERVICEBUS"));
                });

                services.AddSingleton<IEventListener, ServiceBusTopicEventListener>();
                services.AddSingleton<IEventHandler, SendInvoiceHandler>();

                services.AddHostedService<Worker>();
            });
    }
}
