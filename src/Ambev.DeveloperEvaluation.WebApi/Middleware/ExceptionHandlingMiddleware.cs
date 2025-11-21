using FluentValidation;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        _logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Type = "ValidationError";
                response.Error = "One or more validation errors occurred";
                response.Detail = JsonSerializer.Serialize(validationEx.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
                break;

            case DomainException domainEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response.Type = "DomainRuleViolation";
                response.Error = "Business rule violation";
                response.Detail = domainEx.Message;
                break;

            case KeyNotFoundException keyEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Type = "ResourceNotFound";
                response.Error = "Resource not found";
                response.Detail = keyEx.Message;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Type = "AuthenticationError";
                response.Error = "Unauthorized";
                response.Detail = "Access denied";
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Type = "InternalServerError";
                response.Error = "An unexpected error occurred";
                response.Detail = "Please contact support for assistance.";
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }

    private class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}