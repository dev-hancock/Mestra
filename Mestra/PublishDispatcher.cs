namespace Mestra;

using System.Reactive;
using System.Reactive.Linq;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class PublishDispatcher : IPublishDispatcher
{
    private readonly IServiceProvider _services;

    private readonly IPublishStrategy _strategy;

    public PublishDispatcher(IServiceProvider services, IPublishStrategy strategy)
    {
        _services = services;
        _strategy = strategy;
    }

    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>
    {
        if (typeof(TResponse) != typeof(Unit))
        {
            throw new InvalidOperationException($"Expected Unit, got {typeof(TResponse).Name}");
        }

        var handlers = _services.GetServices<IMessageHandler<TMessage, TResponse>>().ToArray();

        if (handlers.Length == 0)
        {
            return Observable.Return(Unit.Default).Select(x => (TResponse)(object)x);
        }

        var adapters = handlers
            .Select(handler => (IMessageHandler<IMessage<Unit>, Unit>)
                Activator.CreateInstance(
                    typeof(PublishHandlerAdapter<>).MakeGenericType(typeof(TMessage)),
                    handler)!
            )
            .ToList();

        return _strategy.Execute(adapters, (IMessage<Unit>)message).Select(x => (TResponse)(object)x);
    }
}