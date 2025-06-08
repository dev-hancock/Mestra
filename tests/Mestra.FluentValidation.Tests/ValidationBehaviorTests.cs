namespace Mestra.FluentValidation.Tests;

using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Abstractions;
using global::FluentValidation;
using global::FluentValidation.Results;
using Moq;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Validation_ShouldPass_WhenNoValidatorsRegistered()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<PingRequest>>();

        var behavior = new ValidationBehavior<PingRequest, string>(validators);

        // Act
        var result = await behavior.Handle(new PingRequest(), Observable.Return("pong"));

        // Assert
        Assert.Equal("pong", result);
    }

    [Fact]
    public async Task Validation_ShouldPass_WhenValidatorsSucceed()
    {
        // Arrange
        var validator = new Mock<IValidator<PingRequest>>();
        validator
            .Setup(x => x.Validate(It.IsAny<ValidationContext<PingRequest>>()))
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<PingRequest, string>([validator.Object]);

        // Act
        var result = await behavior.Handle(new PingRequest(), Observable.Return("pong"));

        // Assert
        Assert.Equal("pong", result);
    }

    [Fact]
    public async Task Validation_ShouldThrow_WhenValidatorFails()
    {
        // Arrange
        var expected = new ValidationResult([
            new ValidationFailure("Property", "Error")
        ]);

        var validator = new Mock<IValidator<PingRequest>>();
        validator
            .Setup(x => x.Validate(It.IsAny<ValidationContext<PingRequest>>()))
            .Returns(expected);

        var behavior = new ValidationBehavior<PingRequest, string>([validator.Object]);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new PingRequest(), Observable.Return("pong")).ToTask());

        Assert.Equal(expected.Errors, ex.Errors);
    }

    public class PingRequest : IRequest<string>;
}