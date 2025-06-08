namespace Mestra.Tests;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Moq;

public class MediatorTests
{
    private readonly Mock<IMessageHandlerFactory> _handlers;

    private readonly Mock<IPipelineFactory> _pipelines;

    private readonly IServiceCollection _services;

    public MediatorTests()
    {
        _services = new ServiceCollection();

        _pipelines = new Mock<IPipelineFactory>();

        _handlers = new Mock<IMessageHandlerFactory>();

        _services
            .AddSingleton<IMediator, Mediator>()
            .AddSingleton<ISendDispatcher, SendDispatcher>()
            .AddSingleton<IPublishDispatcher, PublishDispatcher>()
            .AddSingleton(_pipelines.Object)
            .AddSingleton(_handlers.Object)
            .AddTransient(typeof(IPipeline<,>), typeof(Pipeline<,>))
            .AddTransient<IPublishStrategy, ParallelPublishStrategy>();
    }

    [Fact]
    public async Task Send_ShouldReturnExpectedResult_WhenHandlerRegistered()
    {
        // Arrange
        var scheduler = new TestScheduler();

        var handler = new Mock<IMessageHandler<PingRequest, string>>();
        handler
            .Setup(x => x.Handle(It.IsAny<PingRequest>()))
            .Returns(Observable.Return("pong"));

        _handlers
            .Setup(x => x.GetHandler<PingRequest, string>())
            .Returns(handler.Object);

        _pipelines
            .Setup(x => x.GetPipeline(It.IsAny<PingRequest>()))
            .Returns(() => new Pipeline<PingRequest, string>([]));

        _services.AddTransient<IMessageHandler<PingRequest, string>>(_ => handler.Object);

        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingRequest());

        scheduler.Start();

        // Assert
        Assert.Equal("pong", result);
    }

    [Fact]
    public void Send_ShouldReturnExpectedSequence_WhenHandlerReturnsContinuousObservable()
    {
        // Arrange
        var scheduler = new TestScheduler();

        var handler = new Mock<IMessageHandler<CounterRequest, long>>();
        handler
            .Setup(x => x.Handle(It.IsAny<CounterRequest>()))
            .Returns(Observable
                .Interval(TimeSpan.FromSeconds(1), scheduler)
                .Take(10));

        _handlers
            .Setup(x => x.GetHandler<CounterRequest, long>())
            .Returns(handler.Object);

        _pipelines
            .Setup(x => x.GetPipeline(It.IsAny<CounterRequest>()))
            .Returns(() => new Pipeline<CounterRequest, long>([]));

        _services.AddTransient<IMessageHandler<CounterRequest, long>>(_ => handler.Object);

        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var actual = new List<long>();

        mediator.Send(new CounterRequest()).Subscribe(actual.Add);

        scheduler.AdvanceBy((TimeSpan.FromSeconds(1) * 10).Ticks);
        scheduler.Start();

        // Assert
        var expected = Enumerable.Range(0, 10).Select(x => (long)x);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Publish_ShouldInvokeHandler_WhenSingleHandlerRegistered()
    {
        // Arrange
        var invoked = new[]
        {
            false, false
        };

        void Setup(Mock<IMessageHandler<NotificationEvent, Unit>> handler, int index)
        {
            handler
                .Setup(x => x.Handle(It.IsAny<NotificationEvent>()))
                .Returns(Observable
                    .Return(Unit.Default)
                    .Do(_ => invoked[index] = true));
        }

        var first = new Mock<IMessageHandler<NotificationEvent, Unit>>();
        var second = new Mock<IMessageHandler<NotificationEvent, Unit>>();

        _handlers
            .Setup(x => x.GetHandlers<NotificationEvent, Unit>())
            .Returns([first.Object, second.Object]);

        _pipelines
            .Setup(x => x.GetPipeline(It.IsAny<NotificationEvent>()))
            .Returns(() => new Pipeline<NotificationEvent, Unit>([]));

        Setup(first, 0);
        Setup(second, 1);

        _services.AddTransient<IMessageHandler<NotificationEvent, Unit>>(_ => first.Object);
        _services.AddTransient<IMessageHandler<NotificationEvent, Unit>>(_ => second.Object);

        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new NotificationEvent());

        // Assert
        first.Verify(x => x.Handle(It.IsAny<NotificationEvent>()), Times.Once);
        second.Verify(x => x.Handle(It.IsAny<NotificationEvent>()), Times.Once);

        var expected = new[]
        {
            true, true
        };

        Assert.Equal(expected, invoked);
    }

    [Fact]
    public async Task Send_ShouldThrow_WhenNoHandlerRegistered()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        _pipelines
            .Setup(x => x.GetPipeline(It.IsAny<PingRequest>()))
            .Returns(() => new Pipeline<PingRequest, string>([]));

        // Act
        var ex = await Record.ExceptionAsync(() => mediator.Send(new PingRequest()).ToTask());

        // Assert
        Assert.Contains("No handler was found for request of type PingRequest", ex?.Message);
    }

    [Fact]
    public async Task Publish_ShouldComplete_WhenNoHandlerRegistered()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        _pipelines
            .Setup(x => x.GetPipeline(It.IsAny<NotificationEvent>()))
            .Returns(() => new Pipeline<NotificationEvent, Unit>([]));

        // Act
        var ex = await Record.ExceptionAsync(() => mediator.Publish(new NotificationEvent()).ToTask());

        // Assert
        Assert.Null(ex);
    }
}