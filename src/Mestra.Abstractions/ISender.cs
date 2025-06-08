namespace Mestra.Abstractions;

/// <summary>
///     Sends request messages through the pipeline and returns responses.
/// </summary>
public interface ISender
{
    /// <summary>
    ///     Sends the specified request message.
    /// </summary>
    IObservable<TResponse> Send<TResponse>(IRequest<TResponse> message);
}