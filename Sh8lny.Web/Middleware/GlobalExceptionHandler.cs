using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Sh8lny.Domain.Exceptions;

namespace Sh8lny.Web.Middleware;

/// <summary>
/// Global exception handler middleware for standardized error responses
/// Implements RFC 7807 Problem Details format
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception with correlation ID
        var correlationId = context.TraceIdentifier;
        _logger.LogError(exception, "An error occurred. CorrelationId: {CorrelationId}", correlationId);

        // Determine status code and problem details based on exception type
        var (statusCode, title, detail, errors) = exception switch
        {
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                notFoundEx.Message,
                (IDictionary<string, string[]>?)null
            ),
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validation Error",
                validationEx.Message,
                validationEx.Errors
            ),
            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Forbidden,
                "Forbidden",
                unauthorizedEx.Message,
                (IDictionary<string, string[]>?)null
            ),
            UnauthenticatedException unauthenticatedEx => (
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                unauthenticatedEx.Message,
                (IDictionary<string, string[]>?)null
            ),
            BusinessRuleException businessEx => (
                HttpStatusCode.BadRequest,
                "Business Rule Violation",
                businessEx.Message,
                (IDictionary<string, string[]>?)null
            ),
            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                "Conflict",
                conflictEx.Message,
                (IDictionary<string, string[]>?)null
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred. Please contact support.",
                (IDictionary<string, string[]>?)null
            )
        };

        // Create RFC 7807 Problem Details response
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{(int)statusCode}"
        };

        // Add correlation ID for tracking
        problemDetails.Extensions["traceId"] = correlationId;

        // Add validation errors if present
        if (errors != null && errors.Any())
        {
            problemDetails.Extensions["errors"] = errors;
        }

        // Add stack trace in development
        if (_environment.IsDevelopment() && exception is not DomainException)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        // Set response
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method to register the global exception handler middleware
/// </summary>
public static class GlobalExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandler>();
    }
}
