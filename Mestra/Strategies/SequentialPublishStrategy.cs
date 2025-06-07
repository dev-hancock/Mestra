namespace Mestra.Strategies;

using System.Reactive;
using System.Reactive.Linq;
using Interfaces;

/// <summary>
///     Executes notification handlers sequentially.
/// </summary>
public class SequentialPublishStrategy : IPublishStrategy
{
    /// <inheritdoc />
    public IObservable<Unit> Execute<TMessage>(IEnumerable<IMessageHandler<TMessage, Unit>> handlers, TMessage message)
        where TMessage : IMessage<Unit>
    {
        var merged = handlers
            .Select(h => Observable
                .Defer(() => h.Handle(message))
                .Catch(Observable.Empty<Unit>()))
            .Concat();

        return merged.LastAsync();
    }
}