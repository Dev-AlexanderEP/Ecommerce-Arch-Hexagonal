namespace MixAndMatch.Application.Common;

public record ApiResponse<T>(
    bool Success,
    string? Message,
    T? Data,
    ErrorType? Error = null,
    SuccessType Status = SuccessType.Ok)
{
    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new(true, message, data);

    public static ApiResponse<T> Created(T data, string? message = null) =>
        new(true, message, data, null, SuccessType.Created);

    public static ApiResponse<T> NoContent(string? message = null) =>
        new(true, message, default, null, SuccessType.NoContent);

    public static ApiResponse<T> Fail(string message, ErrorType error = ErrorType.NotFound) =>
        new(false, message, default, error);
}
