using System;
namespace ShoppingApp.Events
{
    public interface IEventListener
    {
        Task StartAsync(string topicName, CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}

