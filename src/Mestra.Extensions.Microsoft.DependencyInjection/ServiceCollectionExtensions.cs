namespace Mestra.Extensions.Microsoft.DependencyInjection;

using Abstractions;
using global::Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for configuring Mestra services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Mestra components to the service collection.
    /// </summary>
    public static IServiceCollection AddMestra(this IServiceCollection services, Action<MestraBuilder>? configure = null)
    {
        var builder = new MestraBuilder(services);

        services.AddTransient(typeof(IPipeline<,>), typeof(Pipeline<,>));

        services.AddTransient<IPublishStrategy, ParallelPublishStrategy>();

        services.AddSingleton<ISendDispatcher, SendDispatcher>();
        services.AddSingleton<IPublishDispatcher, PublishDispatcher>();

        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddSingleton<IPublisher>(sp => sp.GetRequiredService<IMediator>());

        services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
        services.AddTransient<IPipelineFactory, PipelineFactory>();

        configure?.Invoke(builder);

        return services;
    }
}