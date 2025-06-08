namespace Mestra;

using System.Reactive.Linq;
using Abstractions;

/// <summary>
///     Dispatches request messages to their corresponding handlers.
/// </summary>
public class SendDispatcher : ISendDispatcher
{
    private readonly IMessageHandlerFactory _factory;

    public SendDispatcher(IMessageHandlerFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <inheritdoc />
    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>
    {
        return Observable.Defer(() =>
        {
            var handler = _factory.GetHandler<TMessage, TResponse>();

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler was found for request of type {typeof(TMessage).Name}");
            }

            return handler.Handle(message);
        });
    }
}