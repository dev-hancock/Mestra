namespace Mestra;

using Abstractions;

/// <summary>
///     Default implementation of <see cref="IPipelineFactory" /> using IServiceProvider.
/// </summary>
public class PipelineFactory : IPipelineFactory
{
    private readonly IServiceProvider _services;

    public PipelineFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc />
    public IPipeline GetPipeline<TResponse>(IMessage<TResponse> message)
    {
        return (_services.GetService(
            typeof(IPipeline<,>).MakeGenericType(
                message.GetType(),
                typeof(TResponse))
        ) as IPipeline)!;
    }
}