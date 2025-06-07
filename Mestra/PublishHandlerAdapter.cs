namespace Mestra;

using System.Reactive;
using Interfaces;

public class PublishHandlerAdapter<TNotification> : IMessageHandler<IMessage<Unit>, Unit>
    where TNotification : IMessage<Unit>
{
    private readonly IMessageHandler<TNotification, Unit> _inner;

    public PublishHandlerAdapter(IMessageHandler<TNotification, Unit> inner)
    {
        _inner = inner;
    }

    public IObservable<Unit> Handle(IMessage<Unit> message)
    {
        return _inner.Handle((TNotification)message);
    }
}