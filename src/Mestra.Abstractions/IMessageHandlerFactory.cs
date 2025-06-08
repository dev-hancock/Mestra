namespace Mestra.Abstractions;

/// <summary>
///     Provides a factory for resolving message handlers.
/// </summary>
public interface IMessageHandlerFactory
{
    /// <summary>
    ///     Resolves a single message handler.
    /// </summary>
    IMessageHandler<TMessage, TResponse> GetHandler<TMessage, TResponse>() where TMessage : IMessage<TResponse>;

    /// <summary>
    ///     Resolves all registered message handlers.
    /// </summary>
    IEnumerable<IMessageHandler<TMessage, TResponse>> GetHandlers<TMessage, TResponse>() where TMessage : IMessage<TResponse>;
}