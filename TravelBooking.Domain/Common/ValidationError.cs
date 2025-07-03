namespace TravelBooking.Domain.Common;

public sealed record ValidationError : Error
{
    public Error[] Errors { get; }

    public ValidationError(Error[] errors)
        : base("ValidationError", "A validation problem occurred.",ErrorType.Validation)
    {
        Errors = errors;
    }

    public static ValidationError WithErrors(Error[] errors) => new(errors);
}