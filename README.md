# Mestra

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)
[![Code Coverage](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra&metric=coverage)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=dev-hancock_Mestra&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=dev-hancock_Mestra)

![Mestra logo](https://github.com/dev-hancock/Mestra/blob/main/icon.png)

**Mestra** is a minimal, reactive alternative to MediatR for .NET.
It provides simple abstractions for **request/response**, **notifications**, and **pipelines**, built
on [Reactive Extensions (Rx)](https://github.com/dotnet/reactive).

Designed for modern, testable, and highly composable application architectures.

## ✨ Features

* ✅ `Send` and `Publish` abstractions over CQRS-style messaging
* ✅ Customizable `IPipelineBehavior<TMessage, TResponse>` pipeline composition
* ✅ Built on Rx (`IObservable<T>`) for reactive stream support
* ✅ Pluggable with `Microsoft.Extensions.DependencyInjection`
* ✅ Fully testable, no internal threading assumptions
* ✅ Minimal API surface — optimized for control and performance
* ✅ Supports streaming messages (`Range`, `Interval`, etc.)
* ✅ No external dependencies beyond Rx & .NET

## 📦 Packages

| Package                                                                                                                           | Description                                                |
|-----------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------|
| [Mestra](https://www.nuget.org/packages/Mestra)                                                                                   | Core implementation                                        |
| [Mestra.Abstractions](https://www.nuget.org/packages/Mestra.Abstractions)                                                         | Core interfaces & contracts (no implementation)            |
| [Mestra.Extensions.Microsoft.DependencyInjection](https://www.nuget.org/packages/Mestra.Extensions.Microsoft.DependencyInjection) | DI extensions for Microsoft.Extensions.DependencyInjection |
| [Mestra.FluentValidation](https://www.nuget.org/packages/Mestra.FluentValidation)                                                 | Integration with FluentValidation via pipeline behaviors   |

## 📥 Installation

### Minimal core:

```bash
dotnet add package Mestra
```

### With DI support:

```bash
dotnet add package Mestra.Extensions.Microsoft.DependencyInjection
```

### With FluentValidation integration:

```bash
dotnet add package Mestra.FluentValidation
```

## 🚀 Quick Start

### 1️. Register Mestra in DI

```csharp
builder.Services.AddMestra(options =>
{
    options.AddHandlers(typeof(PingRequestHandler));
    options.AddHandlersFromAssembly(typeof(Startup).Assembly);
    options.AddBehaviors(typeof(LoggingBehavior<,>));
});
```

(Optional FluentValidation integration):

```csharp
builder.Services.AddMestraFluentValidation(builder =>
{
    builder.AddValidatorsFromAssembly(typeof(Startup).Assembly);
});
```

### 2️. Define Requests and Handlers

```csharp
public class PingRequest : IRequest<string> { }

public class PingRequestHandler : IMessageHandler<PingRequest, string>
{
    public IObservable<string> Handle(PingRequest request)
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
        return await _mediator.Send(new PingRequest());
    }
}
```

## 🔄 Publish Messages

Mestra supports one-to-many eventing via `Publish`:

```csharp
public class NotificationEvent : INotification { }

public class NotificationEventHandler : IMessageHandler<NotificationEvent, Unit>
{
    public IObservable<Unit> Handle(NotificationEvent notification)
    {
        Console.WriteLine("Notification event received.");
        return Observable.Return(Unit.Default);
    }
}
```

Usage:

```csharp
await _mediator.Publish(new NotificationEvent());
```

## ⚙️ Advanced: Pipelines & Streaming

### Pipeline Behavior Example

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    public IObservable<TResponse> Handle(TRequest request, IObservable<TResponse> next)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        return next.Do(_ => Console.WriteLine($"Handled {typeof(TRequest).Name}"));
    }
}
```

### Streaming Example

```csharp
public class CounterRequest : IRequest<int> { }

public class CounterRequestHandler : IMessageHandler<CounterRequest, int>
{
    public IObservable<int> Handle(CounterRequest request)
    {
        return Observable.Range(1, 3);
    }
}
```

Consume the full stream as a list:

```csharp
var results = await _mediator.Send(new CounterRequest()).ToList();
```

Or subscribe to each item in the stream:

```csharp
var token = _mediator
    .Send(new CounterRequest())
    .Subscribe(count => 
    {
        Console.WriteLine($"Counter: {count}");
    });
```

## 🧪 Testing

Mestra is fully testable — all core abstractions are interface-based and Rx-friendly.

```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Please follow standard C# coding conventions and include tests with PRs.

## 📄 License

Mestra is licensed under the [MIT License](LICENSE).
