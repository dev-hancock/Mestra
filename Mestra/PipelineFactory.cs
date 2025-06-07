namespace Mestra;

using Interfaces;

public class PipelineFactory(IServiceProvider services)
{
    public Pipeline Create<TResponse>(IMessage<TResponse> message)
    {
        var type = typeof(IPipeline<,>).MakeGenericType(message.GetType(), typeof(TResponse));

        if (services.GetService(type) is not Pipeline pipeline)
        {
            throw new Exception($"Unable to find pipeline for {type}");
        }

        return pipeline;
    }
}