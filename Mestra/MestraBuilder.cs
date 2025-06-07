namespace Mestra;

using System.Reflection;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class MestraBuilder
{
    private readonly IServiceCollection _services;

    public MestraBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public MestraBuilder AddBehaviors(params Type[] types)
    {
        foreach (var type in types)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                throw new InvalidOperationException($"{type.Name} must be a concrete class.");
            }

            var interfaces = type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

            foreach (var @interface in interfaces) _services.AddTransient(@interface, type);
        }

        return this;
    }

    public MestraBuilder AddHandlers(params Type[] types)
    {
        foreach (var type in types)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                throw new InvalidOperationException($"{type.Name} must be a concrete class.");
            }

            var interfaces = type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>));

            foreach (var @interface in interfaces) _services.AddTransient(@interface, type);
        }

        return this;
    }

    public MestraBuilder AddHandlersFromAssembly(Assembly assembly)
    {
        var handlers = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>)));

        AddHandlers(handlers.ToArray());

        return this;
    }

    public MestraBuilder UsePublishStrategy<TStrategy>() where TStrategy : class, IPublishStrategy
    {
        _services.AddTransient<IPublishStrategy, TStrategy>();

        return this;
    }
}