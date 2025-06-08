namespace Mestra;

using System.Reactive;
using System.Reactive.Linq;
using Abstractions;

/// <summary>
///     Executes notification handlers in parallel.
/// </summary>
public class ParallelPublishStrategy : IPublishStrategy
{
    /// <inheritdoc />
    public IObservable<Unit> Execute<TMessage>(IEnumerable<IMessageHandler<TMessage, Unit>> handlers, TMessage message)
        where TMessage : IMessage<Unit>
    {
        var merged = handlers
            .Select(h => Observable
                .Defer(() => h.Handle(message))
                .Catch(Observable.Empty<Unit>()))
            .Merge();

        return merged.LastAsync();
    }
}