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
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(context, exception);
        }
        catch (ApplicationExceptionBase exception)
        {
            await HandleApplicationExceptionAsync(context, exception);
        }
        catch(Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            _logger.LogError(exception, "An unhandled exception occurred while processing the request.");
            var response = new ErrorResponse 
            {
                Title = "Internal Server Error",
                Status = 500,
                Error = "Oops! Something went wrong."
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    private static Task HandleApplicationExceptionAsync(HttpContext context, ApplicationExceptionBase exception)
    {
        context.Response.ContentType = "application/json";
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