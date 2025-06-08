namespace Mestra.FluentValidation;

using System.Reflection;
using global::FluentValidation;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides configuration options for Mestra FluentValidation integration.
/// </summary>
public class MestraFluentValidationBuilder
{
    private readonly IServiceCollection _services;

    internal MestraFluentValidationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    ///     Adds validators from the specified assembly.
    /// </summary>
    public MestraFluentValidationBuilder AddValidatorsFromAssembly(Assembly assembly)
    {
        _services.AddValidatorsFromAssembly(assembly);

        return this;
    }

    /// <summary>
    ///     Adds the specified validator types to the service collection.
    /// </summary>
    public MestraFluentValidationBuilder AddValidators(params Type[] validators)
    {
        foreach (var type in validators)
        {
            if (!typeof(IValidator).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"{type.Name} does not implement IValidator.");
            }

            _services.AddTransient(type);
        }

        return this;
    }
}