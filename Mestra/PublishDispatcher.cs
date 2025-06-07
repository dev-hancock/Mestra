namespace Mestra;

using System.Reactive;
using System.Reactive.Linq;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Dispatches notification messages using the configured publish strategy.
/// </summary>
public class PublishDispatcher : IPublishDispatcher
{
    private readonly IServiceProvider _services;

    private readonly IPublishStrategy _strategy;

    public PublishDispatcher(IServiceProvider services, IPublishStrategy strategy)
    {
        _services = services;
        _strategy = strategy;
    }

    /// <inheritdoc />
    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>
    {
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