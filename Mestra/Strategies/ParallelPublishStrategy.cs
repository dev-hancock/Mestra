namespace Mestra.Strategies;

using System.Reactive;
using System.Reactive.Linq;
using Interfaces;

public class ParallelPublishStrategy : IPublishStrategy
{
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