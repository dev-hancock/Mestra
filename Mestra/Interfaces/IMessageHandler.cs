namespace Mestra;

public interface IMessageHandler<in TMessage, out TResponse> where TMessage : IMessage<TResponse>
{
    IObservable<TResponse> Handle(TMessage message);
}