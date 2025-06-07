namespace Mestra.Tests;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Interfaces;
using Microsoft.Reactive.Testing;
using Moq;
using Strategies;

public class PublishStrategyTests
{
    [Fact]
    public async Task Parallel_ShouldInvokeAllHandlers()
    {
        // Arrange
        var scheduler = new TestScheduler();

        var delay = TimeSpan.FromSeconds(1);
        var epsilon = 1L;
        var invoked = new[]
        {
            0L, 0L
        };

        void Setup(Mock<IMessageHandler<NotificationEvent, Unit>> handler, int index)
        {
            handler
                .Setup(x => x.Handle(It.IsAny<NotificationEvent>()))
                .Returns(Observable
                    .Return(Unit.Default)
                    .Delay(delay, scheduler)
                    .Do(_ => invoked[index] = scheduler.Clock));
        }

        var first = new Mock<IMessageHandler<NotificationEvent, Unit>>();
        var second = new Mock<IMessageHandler<NotificationEvent, Unit>>();

        Setup(first, 0);
        Setup(second, 1);

        var strategy = new ParallelPublishStrategy();

        // Act
        var task = strategy.Execute([first.Object, second.Object], new NotificationEvent()).ToTask();

        scheduler.AdvanceBy(delay.Ticks);
        scheduler.Start();

        var result = await task;

        // Assert
        Assert.Equal(Unit.Default, result);
        Assert.All(invoked, x => Assert.InRange(x, delay.Ticks - epsilon, delay.Ticks + epsilon));
    }

    [Fact]
    public async Task Sequential_ShouldInvokeHandlersInOrder()
    {
        // Arrange
        var scheduler = new TestScheduler();

        var delay = TimeSpan.FromSeconds(1);
        var epsilon = 1L;
        var invoked = new[]
        {
            0L, 0L
        };

        void Setup(Mock<IMessageHandler<NotificationEvent, Unit>> handler, int index)
        {
            handler
                .Setup(x => x.Handle(It.IsAny<NotificationEvent>()))
                .Returns(Observable
                    .Return(Unit.Default)
                    .Delay(delay, scheduler)
                    .Do(_ => invoked[index] = scheduler.Clock));
        }

        var first = new Mock<IMessageHandler<NotificationEvent, Unit>>();
        var second = new Mock<IMessageHandler<NotificationEvent, Unit>>();

        Setup(first, 0);
        Setup(second, 1);

        var strategy = new SequentialPublishStrategy();

        // Act
        var task = strategy.Execute([first.Object, second.Object], new NotificationEvent()).ToTask();

        scheduler.AdvanceBy(delay.Ticks);
        scheduler.Start();

        var result = await task;

        // Assert
        Assert.Equal(Unit.Default, result);
        Assert.All(invoked, x => Assert.InRange(x, delay.Ticks - epsilon, delay.Ticks * 2 + epsilon));
    }
}