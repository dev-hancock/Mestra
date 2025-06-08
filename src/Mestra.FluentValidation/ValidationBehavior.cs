namespace Mestra.FluentValidation;

using System.Reactive.Linq;
using Abstractions;
using global::FluentValidation;

/// <summary>
///     Pipeline behavior that performs validation on the request using FluentValidation.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}" /> class.
    /// </summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc />
    public IObservable<TResponse> Handle(TRequest request, IObservable<TResponse> next)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            return Observable.Throw<TResponse>(new ValidationException(failures));
        }

        return next;
    }
}