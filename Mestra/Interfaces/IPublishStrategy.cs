namespace Mestra.Interfaces;

using System.Reactive;

public interface IPublishStrategy
{
    public IObservable<Unit> Execute<TMessage>(IEnumerable<IMessageHandler<TMessage, Unit>> handlers, TMessage message)
        where TMessage : IMessage<Unit>;
}