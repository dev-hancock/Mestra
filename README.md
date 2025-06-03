# Mestra

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra\&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)
[![Code Coverage](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra\&metric=coverage)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra\&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra\&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)

![Mestra logo](https://raw.githubusercontent.com/dev-hancock/Mestra/main/icon.png)

**Mestra** is a minimal, reactive alternative to MediatR for .NET applications. Built on Rx, it enables flexible message dispatching and composition through pipelines and observables—ideal for modern, composable, and testable application architectures.

## ✨ Features

* 🔁 `Send` and `Publish` abstraction over CQRS-style messaging
* ⚙️ Customizable `IPipelineBehavior<,>` support
* 📦 Pluggable with Microsoft.Extensions.DependencyInjection
* 🧪 Fully testable with clean separation of concerns
* 📡 Observable-based API with full Rx support
* 🧵 Zero threading assumptions – caller controls scheduling and subscription

## 📦 Installation

```bash
dotnet add package Mestra
```

> Requires **.NET 8.0 SDK or later**

## 🚀 Quick Start

### 1. Register with Dependency Injection

```csharp
builder.Services.AddMestra(options =>
{
    options.AddHandlers(typeof(PingHandler));
    options.AddBehaviors(typeof(LoggingBehavior<,>));
});
```

### 2. Define Messages and Handlers

```csharp
public class Ping : IMessage<string> { }

public class PingHandler : IMessageHandler<Ping, string>
{
    public IObservable<string> Handle(Ping message)
    {
        return Observable.Return("pong");
    }
}
```

### 3. Use IMediator

```csharp
public class MyService
{
    private readonly IMediator _mediator;

    public MyService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> PingAsync()
    {
        return await _mediator.Send(new Ping());
    }
}
```

## 🔄 Publish Notifications

```csharp
public class MyNotification : INotification { }

public class NotifyHandler : IMessageHandler<MyNotification, Unit>
{
    public IObservable<Unit> Handle(MyNotification notification)
    {
        return Observable.Return(Unit.Default);
    }
}
```

```csharp
await _mediator.Publish(new MyNotification());
```

## ⚙️ Advanced

Mestra supports Rx-based streaming:

```csharp
public class StreamMessage : IMessage<int> { }

public class StreamHandler : IMessageHandler<StreamMessage, int>
{
    public IObservable<int> Handle(StreamMessage message) 
    {
        return Observable.Range(1, 3); 
    }
}
```

## 🧪 Testing

Mestra is designed to be fully testable. You can mock any handler or behavior using Moq or AutoFixture:

```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Please follow standard C# coding conventions and include tests with PRs.

## 📄 License

Licensed under the [MIT License](LICENSE).
