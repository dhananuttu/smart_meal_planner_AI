namespace SmartMealPlanner.Application.Common.Models;

public class ApiResult<T>
{
    public T? Data { get; init; }

    public bool Success { get; init; } = true;

    public static ApiResult<T> Ok(T data) => new() { Data = data, Success = true };
}
