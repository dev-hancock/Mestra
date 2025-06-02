using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Mestra.Tests;

public class MediatorTests
{
    private class Ping : IRequest<string> { }

    private class PingHandler : IMessageHandler<Ping, string>
    {
        public IObservable<string> Handle(Ping message)
        {
            return Observable.Return("pong");
        }
    }

    [Fact]
    public async Task Send_Should_Invoke_Handler_And_Behavior()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddMestra(opt =>
        {
            opt.AddHandlers(typeof(PingHandler));
        });
        
        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();
        
        // Act
        var result = await mediator.Send(new Ping());

        // Assert
        Assert.Equal("pong", result);
    }
}