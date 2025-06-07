namespace Mestra;

using System.Reactive.Linq;
using Interfaces;

/// <summary>
///     Default implementation of a typed message processing pipeline.
/// </summary>
public class Pipeline<TMessage, TResponse> : IPipeline, IPipeline<TMessage, TResponse> where TMessage : IMessage<TResponse>
{
    private readonly IEnumerable<IPipelineBehavior<TMessage, TResponse>> _behaviors;

    public Pipeline(IEnumerable<IPipelineBehavior<TMessage, TResponse>> behaviors)
    {
        _behaviors = behaviors.Reverse();
    }

    /// <inheritdoc />
    public IObservable<object?> Handle(object message, IDispatcher dispatcher)
    {
        return Handle((TMessage)message, dispatcher).Select(x => (object?)x);
    }

    /// <inheritdoc />
    public IObservable<TResponse> Handle(TMessage message, IDispatcher dispatcher)
    {
        var pipeline = _behaviors.Aggregate(
            dispatcher.Dispatch<TMessage, TResponse>(message),
            (next, behavior) => behavior.Handle(message, next)
        );

        return pipeline;
    }
}