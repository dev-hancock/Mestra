namespace Mestra.Abstractions;

/// <summary>
///     Defines a behavior that can be executed as part of the message processing pipeline.
/// </summary>
public interface IPipelineBehavior<in TMessage, TResponse> where TMessage : IMessage<TResponse>
{
    /// <summary>
    ///     Handles the message and optionally delegates to the next behavior in the pipeline.
    /// </summary>
    IObservable<TResponse> Handle(TMessage message, IObservable<TResponse> next);
}