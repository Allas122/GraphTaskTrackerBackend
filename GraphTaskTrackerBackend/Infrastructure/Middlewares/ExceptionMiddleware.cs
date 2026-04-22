using System.Text.Json;
using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.Exceptions;

namespace GraphTaskTrackerBackend.Infrastructure.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (OperationCanceledException)
        { 
            _logger.LogInformation("Request was cancelled (client disconnected).");
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning(exception,
                    "The response has already started. Cannot write error JSON. Connection will be closed.");
                throw;
            }
            if (exception is ValidationException validationException)
            {
                await HandleValidationExceptionAsync(context, validationException);
            }
            else if (exception is ApplicationExceptionBase appException)
            {
                await HandleApplicationExceptionAsync(context, appException);
            }
            else
            {
                await HandleGenericExceptionAsync(context, exception);
            }
        }
    }

    private async Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred while processing the request.");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Title = "Internal Server Error",
            Status = 500,
            Error = "Oops! Something went wrong."
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleApplicationExceptionAsync(HttpContext context, ApplicationExceptionBase exception)
    {
        var (statusCode, title) = exception switch
        {
            AlreadyExistsException => (StatusCodes.Status409Conflict, "Conflict"),
            NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            Unprocessable => (StatusCodes.Status422UnprocessableEntity, "Unprocessable"),
            Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
            Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status400BadRequest, "Application Error")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse
        {
            Title = title,
            Status = statusCode,
            Error = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            );

        var response = new ValidationErrorResponse
        {
            Errors = errors
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}