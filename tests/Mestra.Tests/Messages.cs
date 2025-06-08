namespace Mestra.Tests;

using Abstractions;

public class PingRequest : IRequest<string>;

public class CounterRequest : IRequest<long>;

public class NotificationEvent : INotification;