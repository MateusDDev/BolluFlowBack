namespace BolluFlow.Application.Abstractions;

public class ResultOf<T>
{
    public bool IsSuccessful { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private ResultOf(bool isSuccessful, T? value = default, string? errorMessage = null, int statusCode = 200)
    {
        IsSuccessful = isSuccessful;
        Value = value;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static ResultOf<T> Success(T value) => new(true, value, null, 200);
    public static ResultOf<T> NotFound(string message) => new(false, default, message, 404);
    public static ResultOf<T> BadRequest(string message) => new(false, default, message, 400);
    public static ResultOf<T> Unauthorized(string message) => new(false, default, message, 401);
    public static ResultOf<T> InternalError(string message) => new(false, default, message, 500);
}

public class Result
{
    public bool IsSuccessful { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private Result(bool isSuccessful, string? errorMessage = null, int statusCode = 200)
    {
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static Result Success() => new(true, null, 200);
    public static Result NoContent() => new(true, null, 204);
    public static Result NotFound(string message) => new(false, message, 404);
    public static Result BadRequest(string message) => new(false, message, 400);
}