namespace Mestra.Interfaces;

public interface IPipelineBehavior<in TMessage, TResponse> where TMessage : IMessage<TResponse>
{
    IObservable<TResponse> Handle(TMessage message, IObservable<TResponse> next);
}