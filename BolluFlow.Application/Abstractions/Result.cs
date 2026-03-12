using BolluFlow.Domain.Abstractions;

namespace BolluFlow.Application.Abstractions;

public class ResultOf<T>
{
    public bool IsSuccessful { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private ResultOf(bool isSuccessful, T? value = default, DomainError? error = null, int statusCode = 200)
    {
        IsSuccessful = isSuccessful;
        Value = value;
        ErrorCode = error?.Code;
        ErrorMessage = error?.Message;
        StatusCode = statusCode;
    }

    public static ResultOf<T> Success(T value) => new(true, value);

    public static ResultOf<T> BadRequest(DomainError error) => new(false, default, error, 400);

    public static ResultOf<T> NotFound(DomainError error) => new(false, default, error, 404);

    public static ResultOf<T> Unauthorized(DomainError error) => new(false, default, error, 401);

    public static ResultOf<T> Conflict(DomainError error) => new(false, default, error, 409);

    public static ResultOf<T> Unprocessable(DomainError error) => new(false, default, error, 422);

    public static ResultOf<T> InternalError(DomainError error) => new(false, default, error, 500);
}

public class Result
{
    public bool IsSuccessful { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    private Result(bool isSuccessful, DomainError? error, int statusCode = 200)
    {
        IsSuccessful = isSuccessful;
        ErrorCode = error?.Code;
        ErrorMessage = error?.Message;
        StatusCode = statusCode;
    }

    public static Result Success() => new(true, null);
    public static Result NoContent() => new(true, null, 204);
    public static Result NotFound(DomainError error) => new(false, error, 404);
    public static Result BadRequest(DomainError error) => new(false, error, 400);
    public static Result Conflict(DomainError error) => new(false, error, 409);
    public static Result Unprocessable(DomainError error) => new(false, error, 422);
}