namespace Mestra.Interfaces;

using System.Reactive;

public interface IRequest : IMessage<Unit>;

public interface IRequest<T> : IMessage<T>;