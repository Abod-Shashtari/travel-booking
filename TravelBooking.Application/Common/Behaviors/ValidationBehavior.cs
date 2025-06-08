using FluentValidation;
using MediatR;
using TravelBooking.Domain.Common;

namespace TravelBooking.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TResponse:Result where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);
    
        var errors= _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => new Error(
                Code: $"Validation.{f.PropertyName}",
                Message: f.ErrorMessage,
                Type: ErrorType.Validation
            ))
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
            return await next(cancellationToken);
    
        var validationError = new ValidationError(errors);
        var failureResult = typeof(TResponse)
            .GetMethod("Failure", [typeof(Error)])
            ?.Invoke(null, [validationError]);
        
        return (TResponse)failureResult!;
    }
}
