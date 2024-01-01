using System;
using Azure.Messaging.ServiceBus;
using ShoppingApp.Common.Models;

using Newtonsoft.Json;

namespace ShoppingApp.Events.ServiceBus
{
    public class ServiceBusTopicEventSender : IEventSender
    {
        private readonly ServiceBusClient client;

        public ServiceBusTopicEventSender(ServiceBusClient client)
        {
            this.client = client;
        }

        public async Task SendAsync<T>(string topicName, T tObject) where T : class
        {
            ServiceBusSender sender = client.CreateSender(topicName);

            ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject(tObject));
            await sender.SendMessageAsync(message);
        }
    }
}

