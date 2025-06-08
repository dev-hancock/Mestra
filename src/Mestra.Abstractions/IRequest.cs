namespace Mestra.Abstractions;

using System.Reactive;

/// <summary>
///     Represents a one-way request with no response.
/// </summary>
public interface IRequest : IMessage<Unit>;

/// <summary>
///     Represents a request with an associated response.
/// </summary>
public interface IRequest<T> : IMessage<T>;