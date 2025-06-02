namespace Mestra;

using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;

public class PublishDispatcher : IPublishDispatcher
{
    private readonly IServiceProvider _services;

    public PublishDispatcher(IServiceProvider services)
    {
        _services = services;
    }
    
    public IObservable<TResponse> Dispatch<TMessage, TResponse>(TMessage message) where TMessage :  IMessage<TResponse>
    {
        if (typeof(TResponse) != typeof(Unit))
        {
            throw new InvalidOperationException($"Expected Unit, got {typeof(TResponse).Name}");
        }
        
        var handlers = _services.GetServices<IMessageHandler<TMessage, TResponse>>();

        var merged = handlers
            .Select(h => Observable
                .Defer(() => h.Handle(message))
                .Catch(Observable.Empty<Unit>().Select(x => (TResponse)(object)x)))
            .Merge();

        return merged.LastAsync();
    }
}