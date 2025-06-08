namespace Mestra.Tests;

using System.Reactive.Linq;
using Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class MestraBuilderTests
{
    [Fact]
    public void AddBehaviors_ShouldRegisterPipelineBehavior_WhenConcreteTypeProvided()
    {
        var services = new ServiceCollection();
        var builder = new MestraBuilder(services);

        builder.AddBehaviors(typeof(SamplePipelineBehavior));

        var provider = services.BuildServiceProvider();

        var behavior = provider.GetService<IPipelineBehavior<SampleRequest, SampleResponse>>();

        Assert.NotNull(behavior);
        Assert.IsType<SamplePipelineBehavior>(behavior);
    }

    [Fact]
    public void AddBehaviors_ShouldThrow_WhenAbstractTypeProvided()
    {
        var services = new ServiceCollection();
        var builder = new MestraBuilder(services);

        var ex = Record.Exception(() => builder.AddBehaviors(typeof(AbstractPipelineBehavior)));

        Assert.Contains("must be a concrete class", ex?.Message);
    }

    [Fact]
    public void AddHandlersFromAssembly_ShouldRegisterHandlers_WhenHandlerExistsInAssembly()
    {
        var services = new ServiceCollection();
        var builder = new MestraBuilder(services);

        builder.AddHandlersFromAssembly(typeof(SampleRequestHandler).Assembly);

        var provider = services.BuildServiceProvider();

        var handler = provider.GetService<IMessageHandler<SampleRequest, SampleResponse>>();

        Assert.NotNull(handler);
        Assert.IsType<SampleRequestHandler>(handler);
    }

    [Fact]
    public void AddHandlers_ShouldRegisterHandler_WhenConcreteTypeProvided()
    {
        var services = new ServiceCollection();
        var builder = new MestraBuilder(services);

        builder.AddHandlers(typeof(SampleRequestHandler));

        var provider = services.BuildServiceProvider();

        var handler = provider.GetService<IMessageHandler<SampleRequest, SampleResponse>>();

        Assert.NotNull(handler);
        Assert.IsType<SampleRequestHandler>(handler);
    }

    [Fact]
    public void AddHandlers_ShouldThrow_WhenAbstractTypeProvided()
    {
        var services = new ServiceCollection();
        var builder = new MestraBuilder(services);

        var ex = Record.Exception(() => builder.AddHandlers(typeof(AbstractMessageHandler)));

        Assert.Contains("must be a concrete class", ex?.Message);
    }

    [Fact]
    public void UsePublishStrategy_ShouldRegisterPublishStrategy_WhenConcreteStrategyProvided()
    {
        var services = new ServiceCollection();
        var builder = new MestraBuilder(services);

        builder.UsePublishStrategy<SequentialPublishStrategy>();

        var provider = services.BuildServiceProvider();

        var strategy = provider.GetService<IPublishStrategy>();

        Assert.NotNull(strategy);
        Assert.IsType<SequentialPublishStrategy>(strategy);
    }

    public class SampleRequest : IRequest<SampleResponse>;

    public class SampleResponse;

    public class SampleRequestHandler : IMessageHandler<SampleRequest, SampleResponse>
    {
        public IObservable<SampleResponse> Handle(SampleRequest message)
        {
            return Observable.Empty<SampleResponse>();
        }
    }

    public abstract class AbstractMessageHandler : IMessageHandler<SampleRequest, SampleResponse>
    {
        public IObservable<SampleResponse> Handle(SampleRequest message)
        {
            return Observable.Empty<SampleResponse>();
        }
    }

    public class SamplePipelineBehavior : IPipelineBehavior<SampleRequest, SampleResponse>
    {
        public IObservable<SampleResponse> Handle(SampleRequest message, IObservable<SampleResponse> next)
        {
            return Observable.Empty<SampleResponse>();
        }
    }

    public abstract class AbstractPipelineBehavior : IPipelineBehavior<SampleRequest, SampleResponse>
    {
        public IObservable<SampleResponse> Handle(SampleRequest message, IObservable<SampleResponse> next)
        {
            return Observable.Empty<SampleResponse>();
        }
    }
}