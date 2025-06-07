namespace Mestra;

using System.Reactive.Linq;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class SendDispatcher : ISendDispatcher
{
    private readonly IServiceProvider _services;

    public SendDispatcher(IServiceProvider services)
    {
        _services = services;
    }

    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>
    {
        var handler = _services.GetRequiredService<IMessageHandler<TMessage, TResponse>>();

        return Observable.Defer(() => handler.Handle(message));
    }
}