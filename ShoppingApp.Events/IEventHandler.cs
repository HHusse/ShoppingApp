using System;
namespace ShoppingApp.Events
{
    public interface IEventHandler
    {
        Task HandleAsync<T>(T tObject) where T : class;
    }
}

