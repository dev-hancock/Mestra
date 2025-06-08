namespace Mestra.FluentValidation.Tests;

using Abstractions;
using global::FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class MestraFluentValidationBuilderTests
{
    [Fact]
    public void AddValidatorsFromAssembly_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        var builder = new MestraFluentValidationBuilder(services);

        // Act
        builder.AddValidatorsFromAssembly(typeof(TestValidator).Assembly);

        var provider = services.BuildServiceProvider();

        // Assert
        var validator = provider.GetService<IValidator<TestRequest>>();

        Assert.NotNull(validator);
        Assert.IsType<TestValidator>(validator);
    }

    [Fact]
    public void AddValidators_ShouldRegisterSpecifiedValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        var builder = new MestraFluentValidationBuilder(services);

        // Act
        builder.AddValidators(typeof(TestValidator));

        var provider = services.BuildServiceProvider();

        // Assert
        var validator = provider.GetService<TestValidator>();

        Assert.NotNull(validator);
    }

    [Fact]
    public void AddValidators_ShouldThrow_WhenTypeDoesNotImplementIValidator()
    {
        // Arrange
        var services = new ServiceCollection();

        var builder = new MestraFluentValidationBuilder(services);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            builder.AddValidators(typeof(object)));

        Assert.Contains("does not implement IValidator", ex.Message);
    }

    public class TestRequest : IRequest<string>;

    public class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x).NotNull();
        }
    }
}