namespace Mestra;

using System.Reactive;
using System.Reactive.Linq;
using Abstractions;

/// <summary>
///     Dispatches notification messages using the configured publish strategy.
/// </summary>
public class PublishDispatcher : IPublishDispatcher
{
    private readonly IMessageHandlerFactory _factory;

    private readonly IPublishStrategy _strategy;

    public PublishDispatcher(IMessageHandlerFactory factory, IPublishStrategy strategy)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    /// <inheritdoc />
    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>
    {
        var handlers = _factory.GetHandlers<TMessage, TResponse>().ToArray();

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