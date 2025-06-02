namespace Mestra;

using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;

public class Mediator : IMediator
{
    private readonly ConcurrentDictionary<Type, Pipeline> _pipelines = new();

    private readonly IPublishDispatcher _publish;

    private readonly ISendDispatcher _send;
    
    private readonly PipelineFactory _factory;

    public Mediator(PipelineFactory factory, IPublishDispatcher publish, ISendDispatcher send)
    {
        _factory = factory;
        _publish = publish;
        _send = send;
    }

    public IObservable<TResponse> Send<TResponse>(IRequest<TResponse> message)
    {
        var type = message.GetType();

        var pipeline = _pipelines.GetOrAdd(type,  _ => _factory.Create(message));
        
        return pipeline.Handle(message, _send).Select(x => (TResponse)x!);
    }

    public IObservable<Unit> Publish(INotification notification)
    {
        var type = notification.GetType();

        var pipeline = _pipelines.GetOrAdd(type, _ => _factory.Create(notification));

        return pipeline.Handle(notification, _publish).Select(x => (Unit)x!);
    }
}