namespace Mestra.FluentValidation;

using Abstractions;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods to register FluentValidation-based pipeline behaviors.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers <see cref="ValidationBehavior{TRequest, TResponse}" /> as a pipeline behavior
    ///     and provides a builder to configure additional FluentValidation-related options.
    /// </summary>
    public static IServiceCollection AddMestraFluentValidation(this IServiceCollection services,
        Action<MestraFluentValidationBuilder>? configure = null)
    {
        var builder = new MestraFluentValidationBuilder(services);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        configure?.Invoke(builder);

        return services;
    }
}