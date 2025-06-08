namespace Mestra.Abstractions;

/// <summary>
///     Defines a dispatcher for sending messages through the Mestra pipeline.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    ///     Dispatches a message through the pipeline and returns an observable sequence of responses.
    /// </summary>
    IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage : IMessage<TResponse>;
}