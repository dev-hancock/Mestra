namespace Mestra.Tests;

using System.Reactive.Linq;
using Interfaces;
using Moq;

public class PipelineBehaviorTests
{
    [Fact]
    public async Task Pipeline_ShouldInvokeDispatcher_WhenNoBehaviors()
    {
        // Arrange
        var dispatcher = new Mock<IDispatcher>();
        dispatcher
            .Setup(x => x.Dispatch<PingRequest, string>(It.IsAny<PingRequest>()))
            .Returns(Observable.Return("pong"));

        var pipeline = new Pipeline<PingRequest, string>([]);

        // Act
        var result = await pipeline.Handle(new PingRequest(), dispatcher.Object);

        // Assert
        dispatcher.Verify(x => x.Dispatch<PingRequest, string>(It.IsAny<PingRequest>()), Times.Once);

        Assert.Equal("pong", result);
    }


    [Fact]
    public async Task Pipeline_ShouldInvokeBehaviorsInOrder()
    {
        // Arrange
        var log = new List<string>();

        IObservable<string> Middleware(string name, IObservable<string> next)
        {
            return Observable.Defer(() =>
            {
                log.Add($"{name}-before");

                return next.Do(_ => log.Add($"{name}-after"));
            });
        }

        var outer = new Mock<IPipelineBehavior<PingRequest, string>>();
        outer
            .Setup(x => x.Handle(It.IsAny<PingRequest>(), It.IsAny<IObservable<string>>()))
            .Returns((PingRequest _, IObservable<string> next) =>
                Middleware("outer", next));

        var inner = new Mock<IPipelineBehavior<PingRequest, string>>();
        inner
            .Setup(x => x.Handle(It.IsAny<PingRequest>(), It.IsAny<IObservable<string>>()))
            .Returns((PingRequest _, IObservable<string> next) =>
                Middleware("inner", next));

        var dispatcher = new Mock<IDispatcher>();
        dispatcher
            .Setup(x => x.Dispatch<PingRequest, string>(It.IsAny<PingRequest>()))
            .Returns(Observable
                .Return("pong")
                .Do(_ => log.Add("dispatcher")));

        var pipeline = new Pipeline<PingRequest, string>([
            outer.Object,
            inner.Object
        ]);

        // Act
        var result = await pipeline.Handle(new PingRequest(), dispatcher.Object);

        // Assert
        var expected = new[]
        {
            "outer-before", "inner-before", "dispatcher", "inner-after", "outer-after"
        };

        Assert.Equal("pong", result);
        Assert.Equal(expected, log);
    }
}