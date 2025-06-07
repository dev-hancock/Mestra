namespace Mestra;

using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Strategies;

public static class DependencyInjection
{
    public static IServiceCollection AddMestra(this IServiceCollection services, Action<MestraBuilder>? configure = null)
    {
        var builder = new MestraBuilder(services);

        services.AddSingleton<PipelineFactory>();
        services.AddTransient(typeof(IPipeline<,>), typeof(Pipeline<,>));

        services.AddTransient<IPublishStrategy, ParallelPublishStrategy>();

        services.AddSingleton<ISendDispatcher, SendDispatcher>();
        services.AddSingleton<IPublishDispatcher, PublishDispatcher>();

        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddSingleton<IPublisher>(sp => sp.GetRequiredService<IMediator>());

        configure?.Invoke(builder);

        return services;
    }
}