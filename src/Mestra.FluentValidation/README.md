# Mestra.FluentValidation

![Mestra logo](https://github.com/dev-hancock/Mestra/blob/main/icon.png)

**Mestra.FluentValidation** provides integration between [Mestra](https://github.com/dev-hancock/Mestra)
and [FluentValidation](https://fluentvalidation.net/).

It allows you to automatically register FluentValidation validators as Mestra pipeline behaviors, enabling validation of
Mestra requests automatically as part of the pipeline.

## 📥 Installation

```bash
dotnet add package Mestra.FluentValidation
```

> Requires **.NET 8.0** or later.

## 📚 Package Contents

* `IServiceCollection.AddMestraFluentValidation(...)` extension method
* `MestraFluentValidationBuilder` configuration object
* Registers:
    * `ValidationBehavior<TRequest, TResponse>` as an `IPipelineBehavior<TRequest, TResponse>`

## 🚀 Usage

```csharp
services.AddMestra(builder =>
{
    builder.AddValidators(typeof(MyRequestValidator));
    builder.AddValidatorsFromAssembly(typeof(Startup).Assembly);
});
```

## 🔗 Usage Scenarios

* Apply FluentValidation validators automatically to Mestra requests
* Compose validation into your Mestra pipelines
* Centralize validation concerns without extra code in your handlers

## Example

```csharp
public class MyRequest : IRequest<string> { }

public class MyRequestValidator : AbstractValidator<MyRequest>
{
    public MyRequestValidator()
    {
        RuleFor(x => x).NotNull();
    }
}
```

When `IMediator.Send(new MyRequest())` is called, the `MyRequestValidator` will be executed automatically via the
`ValidationBehavior`.

If validation fails, a `ValidationException` will be thrown.
