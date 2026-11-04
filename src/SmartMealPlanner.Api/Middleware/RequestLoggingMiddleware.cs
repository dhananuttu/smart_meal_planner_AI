namespace SmartMealPlanner.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("HTTP {Method} {Path} started", method, path);

        await _next(context);

        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode}",
            method,
            path,
            context.Response.StatusCode);
    }
}
