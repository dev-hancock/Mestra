namespace Mestra;

using System.Reactive;
using Interfaces;

/// <summary>
///     Adapts a typed notification handler to the generic publish pipeline.
/// </summary>
internal class PublishHandlerAdapter<TNotification> : IMessageHandler<IMessage<Unit>, Unit>
    where TNotification : IMessage<Unit>
{
    private readonly IMessageHandler<TNotification, Unit> _inner;

    public PublishHandlerAdapter(IMessageHandler<TNotification, Unit> inner)
    {
        _inner = inner;
    }

    /// <inheritdoc />
    public IObservable<Unit> Handle(IMessage<Unit> message)
    {
        return _inner.Handle((TNotification)message);
    }
}