namespace Mestra;

using System.Reactive.Linq;
using Interfaces;

public abstract class Pipeline
{
    public abstract IObservable<object?> Handle(object message, IDispatcher dispatcher);
}

public class Pipeline<TMessage, TResponse> : Pipeline, IPipeline<TMessage, TResponse> where TMessage : IMessage<TResponse>
{
    private readonly IEnumerable<IPipelineBehavior<TMessage, TResponse>> _behaviors;

    public Pipeline(IEnumerable<IPipelineBehavior<TMessage, TResponse>> behaviors)
    {
        _behaviors = behaviors.Reverse();
    }

    public IObservable<TResponse> Handle(TMessage message, IDispatcher dispatcher)
    {
        var pipeline = _behaviors.Aggregate(
            dispatcher.Dispatch<TMessage, TResponse>(message),
            (next, behavior) => behavior.Handle(message, next)
        );

        return pipeline;
    }

    public override IObservable<object?> Handle(object message, IDispatcher dispatcher)
    {
        return Handle((TMessage)message, dispatcher).Select(x => (object?)x);
    }
}