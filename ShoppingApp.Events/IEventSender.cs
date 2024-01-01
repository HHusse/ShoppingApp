using System;
namespace ShoppingApp.Events
{
    public interface IEventSender
    {
        Task SendAsync<T>(string topicName, T tObject) where T : class;
    }
}

