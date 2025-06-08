namespace Mestra.Abstractions;

/// <summary>
///     Provides a factory for resolving message pipelines.
/// </summary>
public interface IPipelineFactory
{
    /// <summary>
    ///     Resolves the pipeline for the given message type.
    /// </summary>
    IPipeline GetPipeline<TResponse>(IMessage<TResponse> message);
}