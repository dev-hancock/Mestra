namespace Mestra;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddMestra(this IServiceCollection services, Action<MestraOptions> configure)
    {
        var options = new MestraOptions(services);
        
        configure(options);
        
        services.AddSingleton<PipelineFactory>();
        services.AddTransient(typeof(IPipeline<,>), typeof(Pipeline<,>));
        
        services.AddSingleton<ISendDispatcher, SendDispatcher>();
        services.AddSingleton<IPublishDispatcher, PublishDispatcher>();
        
        services.AddSingleton<IMediator, Mediator>();
        
        return services;
    }
}

public class MestraOptions
{
    private readonly IServiceCollection _services;

    public MestraOptions(IServiceCollection services)
    {
        _services = services;
    }
    
    public MestraOptions AddBehaviors(params Type[] types)
    {
        foreach (var type in types)
        {
            if (!type.IsGenericTypeDefinition && !ImplementsOpenGeneric(type, typeof(IPipelineBehavior<,>)))
            {
                throw new InvalidOperationException($"{type.Name} does not implement IPipelineBehavior");
            }

            _services.AddTransient(typeof(IPipelineBehavior<,>), type);
        }
        
        return this;
    }

    public MestraOptions AddHandlers(params Type[] types)
    {
        foreach (var impl in types)
        {
            if (!impl.IsClass || impl.IsAbstract)
            {
                throw new InvalidOperationException($"{impl.Name} must be a concrete class.");
            }
            
            var interfaces = impl
                .GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>));

            foreach (var iface in interfaces)
            {
                _services.AddTransient(iface, impl);
            }
        }
        
        return this;
    }
    
    private static bool ImplementsOpenGeneric(Type type, Type generic)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == generic);
    }
}