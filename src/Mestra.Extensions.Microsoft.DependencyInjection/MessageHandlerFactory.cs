namespace Mestra.Extensions.Microsoft.DependencyInjection;

using Abstractions;
using global::Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Default implementation of <see cref="IMessageHandlerFactory" /> using IServiceProvider.
/// </summary>
public class MessageHandlerFactory : IMessageHandlerFactory
{
    private readonly IServiceProvider _services;

    public MessageHandlerFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc />
    public IMessageHandler<TMessage, TResponse> GetHandler<TMessage, TResponse>() where TMessage : IMessage<TResponse>
    {
        return _services.GetRequiredService<IMessageHandler<TMessage, TResponse>>();
    }

    /// <inheritdoc />
    public IEnumerable<IMessageHandler<TMessage, TResponse>> GetHandlers<TMessage, TResponse>() where TMessage : IMessage<TResponse>
    {
        return _services.GetServices<IMessageHandler<TMessage, TResponse>>();
    }
}