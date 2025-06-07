namespace Mestra.Interfaces;

using System.Reactive;

/// <summary>
///     Defines a strategy for executing multiple notification handlers.
/// </summary>
public interface IPublishStrategy
{
    /// <summary>
    ///     Executes the handlers for the given notification message.
    /// </summary>
    public IObservable<Unit> Execute<TMessage>(IEnumerable<IMessageHandler<TMessage, Unit>> handlers, TMessage message)
        where TMessage : IMessage<Unit>;
}