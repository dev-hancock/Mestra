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

        configure?.Invoke(builder);

        services.Add(new ServiceDescriptor(typeof(IPipeline<,>), typeof(Pipeline<,>), builder.Lifetime));

        services.Add(new ServiceDescriptor(typeof(IPublishStrategy), typeof(ParallelPublishStrategy), builder.Lifetime));
        services.Add(new ServiceDescriptor(typeof(IPublishStrategy), typeof(SequentialPublishStrategy), builder.Lifetime));

        services.Add(new ServiceDescriptor(typeof(ISendDispatcher), typeof(SendDispatcher), builder.Lifetime));
        services.Add(new ServiceDescriptor(typeof(IPublishDispatcher), typeof(PublishDispatcher), builder.Lifetime));

        services.Add(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), builder.Lifetime));

        services.Add(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), builder.Lifetime));
        services.Add(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), builder.Lifetime));

        services.Add(new ServiceDescriptor(typeof(IMessageHandlerFactory), typeof(MessageHandlerFactory), builder.Lifetime));
        services.Add(new ServiceDescriptor(typeof(IPipelineFactory), typeof(PipelineFactory), builder.Lifetime));

        return services;
    }
}