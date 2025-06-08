namespace Mestra.Abstractions;

using System.Reactive;

/// <summary>
///     Publishes notification messages to all registered handlers.
/// </summary>
public interface IPublisher
{
    /// <summary>
    ///     Publishes the specified notification.
    /// </summary>
    IObservable<Unit> Publish(INotification notification);
}