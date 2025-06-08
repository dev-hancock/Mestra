namespace Mestra;

using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using Abstractions;

/// <summary>
///     Default implementation of the Mestra mediator.
/// </summary>
public class Mediator : IMediator
{
    private readonly IPipelineFactory _factory;

    private readonly ConcurrentDictionary<Type, IPipeline> _pipelines = new();

    private readonly IPublishDispatcher _publish;

    private readonly ISendDispatcher _send;

    public Mediator(IPipelineFactory factory, IPublishDispatcher publish, ISendDispatcher send)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _publish = publish ?? throw new ArgumentNullException(nameof(publish));
        _send = send ?? throw new ArgumentNullException(nameof(send));
    }

    /// <inheritdoc />
    public IObservable<TResponse> Send<TResponse>(IRequest<TResponse> message)
    {
        var type = message.GetType();

        var pipeline = _pipelines.GetOrAdd(type, _ => _factory.GetPipeline(message));

        return pipeline.Handle(message, _send).Select(x => (TResponse)x!);
    }

    /// <inheritdoc />
    public IObservable<Unit> Publish(INotification notification)
    {
        var type = notification.GetType();

        var pipeline = _pipelines.GetOrAdd(type, _ => _factory.GetPipeline(notification));

        return pipeline.Handle(notification, _publish).Select(x => (Unit)x!);
    }
}