namespace Mestra.Interfaces;

using System.Reactive;

public interface IPublisher
{
    IObservable<Unit> Publish(INotification notification);
}