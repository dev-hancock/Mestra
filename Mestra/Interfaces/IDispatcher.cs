namespace Mestra.Interfaces;

public interface IDispatcher
{
    IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>;
}