namespace BolluFlow.Domain.Abstractions;

public class DomainResult<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private DomainResult(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private DomainResult(string error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static DomainResult<T> Success(T value) => new(value);
    public static DomainResult<T> Failure(string error) => new(error);
    
    public T Value => IsSuccess 
        ? field! 
        : throw new InvalidOperationException("Cannot get Value when result is failure");

    public string Error => !IsSuccess 
        ? field! 
        : throw new InvalidOperationException("Cannot get Error when result is success");
}


public class DomainResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private DomainResult(bool success, string? error)
    {
        IsSuccess = success;
        Error = error;
    }

    public static DomainResult Success()
        => new(true, null);

    public static DomainResult Failure(string error)
        => new(false, error);

    public string Error => IsFailure
        ? field!
        : throw new InvalidOperationException(
            "Cannot get Error when result is success");
}
