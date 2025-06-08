# Mestra.Extensions.Microsoft.DependencyInjection

![Mestra logo](https://github.com/dev-hancock/Mestra/blob/main/icon.png)

**Mestra.Extensions.Microsoft.DependencyInjection** provides an integration package to register Mestra into [
`Microsoft.Extensions.DependencyInjection`](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).

It enables seamless registration of Mestra’s mediator services and pipeline behaviors in .NET applications.

## 📦 Installation

```bash
dotnet add package Mestra.Extensions.Microsoft.DependencyInjection
```

> Requires **.NET 8.0** or later.

## 📚 Package Contents

* `IServiceCollection.AddMestra(...)` extension method
* `MestraBuilder` configuration object
* Registers:

    * `IMediator`
    * `ISender`
    * `IPublisher`
    * `ISendDispatcher`
    * `IPublishDispatcher`
    * `IPipelineFactory`
    * `IMessageHandlerFactory`
    * `IPipeline<TMessage, TResponse>`
    * `IPublishStrategy` (default: Parallel)

## 🚀 Usage

```csharp
services.AddMestra(builder =>
{
    builder.AddHandlers(typeof(MyHandler));
    builder.AddBehaviors(typeof(MyBehavior<,>));
    builder.AddHandlersFromAssembly(typeof(Startup).Assembly);
});
```

## 🔗 Usage Scenarios

* Register Mestra mediator services in ASP.NET Core applications
* Register handlers and pipeline behaviors for CQRS-based messaging
* Plug in custom publish strategies and factories
* Support clean modular registration in DI-based apps


