using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartMealPlanner.Domain.Exceptions;
using DomainValidationException = SmartMealPlanner.Domain.Exceptions.ValidationException;

namespace SmartMealPlanner.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problem) = exception switch
        {
            DomainValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    validationEx.Message,
                    validationEx.Errors)),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    notFoundEx.Message)),

            ForbiddenException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    forbiddenEx.Message)),

            UnauthorizedException unauthorizedEx => (
                StatusCodes.Status401Unauthorized,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    unauthorizedEx.Message)),

            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    conflictEx.Message)),

            FluentValidation.ValidationException fluentEx => (
                StatusCodes.Status400BadRequest,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    fluentEx.Message,
                    fluentEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))),

            _ => (
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred."))
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        IDictionary<string, string[]>? errors = null)
    {
        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors is not null && errors.Count > 0)
        {
            problem.Extensions["errors"] = errors;
        }

        return problem;
    }
}
