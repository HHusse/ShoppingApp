using System;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using ShoppingApp.Common.Models;

namespace ShoppingApp.Events.ServiceBus
{
    public class ServiceBusTopicEventListener : IEventListener
    {
        private readonly ServiceBusClient client;
        private ServiceBusProcessor? processor;
        private readonly IEventHandler handler;

        public ServiceBusTopicEventListener(ServiceBusClient client, IEventHandler handler)
        {
            this.client = client;
            this.handler = handler;
        }

        public async Task StartAsync(string topicName, CancellationToken cancellationToken)
        {
            processor = client.CreateProcessor(topicName, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await processor.StopProcessingAsync(cancellationToken);
            processor.ProcessMessageAsync -= MessageHandler;
            processor.ProcessErrorAsync -= ErrorHandler;
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            InvoiceDetails invoice = JsonConvert.DeserializeObject<InvoiceDetails>(body);

            await handler.HandleAsync(invoice);
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occurred: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}

