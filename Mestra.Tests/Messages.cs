namespace Mestra.Tests;

using Interfaces;

public class PingRequest : IRequest<string>;

public class CounterRequest : IRequest<long>;

public class NotificationEvent : INotification;