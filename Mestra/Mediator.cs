namespace Mestra;

using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Default implementation of the Mestra mediator.
/// </summary>
public class Mediator : IMediator
{
    private readonly ConcurrentDictionary<Type, IPipeline> _pipelines = new();

    private readonly IPublishDispatcher _publish;

    private readonly ISendDispatcher _send;

    private readonly IServiceProvider _services;

    public Mediator(IServiceProvider services, IPublishDispatcher publish, ISendDispatcher send)
    {
        _services = services;
        _publish = publish;
        _send = send;
    }

    /// <inheritdoc />
    public IObservable<TResponse> Send<TResponse>(IRequest<TResponse> message)
    {
        var type = message.GetType();

        var pipeline = _pipelines.GetOrAdd(type, _ => GetPipeline(message));

        return pipeline.Handle(message, _send).Select(x => (TResponse)x!);
    }

    /// <inheritdoc />
    public IObservable<Unit> Publish(INotification notification)
    {
        var type = notification.GetType();

        var pipeline = _pipelines.GetOrAdd(type, _ => GetPipeline(notification));

        return pipeline.Handle(notification, _publish).Select(x => (Unit)x!);
    }

    private IPipeline GetPipeline<TResponse>(IMessage<TResponse> message)
    {
        return (_services.GetRequiredService(
            typeof(IPipeline<,>).MakeGenericType(
                message.GetType(),
                typeof(TResponse))
        ) as IPipeline)!;
    }
}