namespace Mestra.Interfaces;

using System.Reactive;

/// <summary>
///     Handles a one-way message with no response.
/// </summary>
public interface IMessageHandler<in TMessage> : IMessageHandler<TMessage, Unit> where TMessage : IMessage<Unit>;

/// <summary>
///     Handles a message and produces a response.
/// </summary>
public interface IMessageHandler<in TMessage, out TResponse> where TMessage : IMessage<TResponse>
{
    /// <summary>
    ///     Handles the specified message.
    /// </summary>
    IObservable<TResponse> Handle(TMessage message);
}