namespace Mestra;

using System.Reactive;

public interface IPublisher
{
    IObservable<Unit> Publish(INotification notification);
}