namespace Mestra.Interfaces;

using System.Reactive;

public interface IMessageHandler<in TMessage> : IMessageHandler<TMessage, Unit> where TMessage : IMessage<Unit>;

public interface IMessageHandler<in TMessage, out TResponse> where TMessage : IMessage<TResponse>
{
    IObservable<TResponse> Handle(TMessage message);
}