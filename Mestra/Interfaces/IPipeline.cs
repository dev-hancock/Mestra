namespace Mestra;

public interface IPipeline<in TMessage, out TResponse> where TMessage : IMessage<TResponse>
{
    IObservable<TResponse> Handle(TMessage message, IDispatcher dispatcher);
}