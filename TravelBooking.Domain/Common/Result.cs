namespace TravelBooking.Domain.Common;

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; private set; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);

    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error!);
    }
}