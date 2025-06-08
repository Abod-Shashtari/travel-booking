namespace TravelBooking.Domain.Common;
public class Result<T>:Result
{
    public T? Value { get; set; }
    
    private Result(bool isSuccess, T? value, Error? error):base(isSuccess,error)
    {
        Value = value;
    }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public new static Result<T?> Failure(Error error) => new(false, default, error);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}
