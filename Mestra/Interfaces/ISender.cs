namespace Mestra;

public interface ISender
{
    IObservable<TResponse> Send<TResponse>(IRequest<TResponse> message);
}