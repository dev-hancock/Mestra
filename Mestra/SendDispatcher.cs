namespace Mestra;

using System.Reactive.Linq;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Dispatches request messages to their corresponding handlers.
/// </summary>
public class SendDispatcher : ISendDispatcher
{
    private readonly IServiceProvider _services;

    public SendDispatcher(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc />
    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>
    {
        return Observable.Defer(() =>
        {
            var handler = _services.GetService<IMessageHandler<TMessage, TResponse>>();

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler was found for request of type {typeof(TMessage).Name}");
            }

            return handler.Handle(message);
        });
    }
}