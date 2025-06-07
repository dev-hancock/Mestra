namespace Mestra.Interfaces;

/// <summary>
///     Represents a generic message processing pipeline.
/// </summary>
public interface IPipeline
{
    /// <summary>
    ///     Processes a message through the pipeline.
    /// </summary>
    IObservable<object?> Handle(object message, IDispatcher dispatcher);
}

/// <summary>
///     Defines a pipeline for processing a message through a sequence of behaviors and dispatching it.
/// </summary>
public interface IPipeline<in TMessage, out TResponse> where TMessage : IMessage<TResponse>
{
    /// <summary>
    ///     Processes the message through the pipeline.
    /// </summary>
    IObservable<TResponse> Handle(TMessage message, IDispatcher dispatcher);
}