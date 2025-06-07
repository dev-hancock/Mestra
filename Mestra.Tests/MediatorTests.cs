namespace Mestra.Tests;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using Moq;

public class MediatorTests
{
    [Fact]
    public async Task Send_ShouldReturnExpectedResult_WhenHandlerRegistered()
    {
        // Arrange
        var services = new ServiceCollection().AddMestra();

        var scheduler = new TestScheduler();

        var handler = new Mock<IMessageHandler<PingRequest, string>>();
        handler
            .Setup(x => x.Handle(It.IsAny<PingRequest>()))
            .Returns(Observable.Return("pong"));

        services.AddTransient<IMessageHandler<PingRequest, string>>(_ => handler.Object);

        var provider = services.BuildServiceProvider();
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
        var services = new ServiceCollection().AddMestra();

        var scheduler = new TestScheduler();

        var handler = new Mock<IMessageHandler<CounterRequest, long>>();
        handler
            .Setup(x => x.Handle(It.IsAny<CounterRequest>()))
            .Returns(Observable
                .Interval(TimeSpan.FromSeconds(1), scheduler)
                .Take(10));

        services.AddTransient<IMessageHandler<CounterRequest, long>>(_ => handler.Object);

        var provider = services.BuildServiceProvider();
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
        var services = new ServiceCollection().AddMestra();

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

        Setup(first, 0);
        Setup(second, 1);

        services.AddTransient<IMessageHandler<NotificationEvent, Unit>>(_ => first.Object);
        services.AddTransient<IMessageHandler<NotificationEvent, Unit>>(_ => second.Object);

        var provider = services.BuildServiceProvider();
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
        var services = new ServiceCollection().AddMestra();

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Record.ExceptionAsync(() => mediator.Send(new PingRequest()).ToTask());

        // Assert
        Assert.Contains("No handler was found for request of type PingRequest", ex?.Message);
    }

    [Fact]
    public async Task Publish_ShouldComplete_WhenNoHandlerRegistered()
    {
        // Arrange
        var services = new ServiceCollection().AddMestra();

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var ex = await Record.ExceptionAsync(() => mediator.Publish(new NotificationEvent()).ToTask());

        // Assert
        Assert.Null(ex);
    }

    public class PingRequest : IRequest<string>;

    public class CounterRequest : IRequest<long>;

    public class NotificationEvent : INotification;
}