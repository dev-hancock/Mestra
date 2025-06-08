# Mestra.Abstractions

![Mestra logo](https://github.com/dev-hancock/Mestra/blob/main/icon.png)

**Mestra.Abstractions** defines the core contracts and interfaces for
the [Mestra](https://github.com/dev-hancock/Mestra) reactive mediator library.

It allows other projects to depend on Mestra abstractions without requiring a specific implementation.

Typical consumers of this package include:

* Shared message contracts libraries
* Application core/domain layers (DDD style)
* Pipeline behaviors
* Cross-cutting libraries
* Third-party extensions to Mestra

## 📥 Installation

```bash
dotnet add package Mestra.Abstractions
```

> Requires **.NET Standard 2.1** or later.
> Compatible with .NET Core 3.0+, .NET 5+, .NET 6+, .NET 7+, .NET 8+, .NET 9+.

## 📚 Package Contents

Core messaging interfaces:

* `IMessage<TResponse>`
* `IRequest<TResponse>`
* `INotification`

Handler interfaces:

* `IMessageHandler<TMessage, TResponse>`
* `ISender`
* `IPublisher`

Dispatcher abstractions:

* `IDispatcher`
* `ISendDispatcher`
* `IPublishDispatcher`

Pipeline and behaviors:

* `IPipeline<TMessage, TResponse>`
* `IPipelineBehavior<TMessage, TResponse>`
* `IPipelineFactory`
* `IPublishStrategy`

Support:

* `IMediator`
* `IMessageHandlerFactory`

## 🚀 Usage

Consumers should depend on **Mestra.Abstractions** when defining:

* Application or domain-level message contracts
* Cross-cutting pipeline behaviors
* Shared validation or logging behaviors
* Reusable extensions or conventions for Mestra

## 📝 Example

```csharp
public class PingRequest : IRequest<string> { }

public class PingHandler : IMessageHandler<PingRequest, string>
{
    public IObservable<string> Handle(PingRequest request)
    {
        return Observable.Return("pong");
    }
}
```

## ❓ Why separate Abstractions?

Separating **Mestra.Abstractions** allows:

* Clean decoupling between API definitions and runtime implementations
* Consumers to reference a minimal package with no dependency on Microsoft.Extensions.DependencyInjection
* Framework and library authors to extend Mestra in their own way
