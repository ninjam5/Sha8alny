namespace Sh8lny.Shared.DTOs.Common;

/// <summary>
/// Generic service response wrapper for consistent API responses.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ServiceResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResponse<T> Success(T data, string? message = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static ServiceResponse<T> Failure(string message, List<string>? errors = null)
    {
        return new ServiceResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
