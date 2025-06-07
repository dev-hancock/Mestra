namespace Mestra.Interfaces;

using System.Reactive;

/// <summary>
/// Represents a one-way notification message.
/// </summary>
public interface INotification : IMessage<Unit>;