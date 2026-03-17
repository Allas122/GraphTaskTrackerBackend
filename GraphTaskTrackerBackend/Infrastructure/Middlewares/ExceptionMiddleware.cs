using System.Text.Json;
using FluentValidation;
using GraphTaskTrackerBackend.Api.Models;
using GraphTaskTrackerBackend.Application.Exceptions;

namespace GraphTaskTrackerBackend.Infrastructure.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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
    }

    private static Task HandleApplicationExceptionAsync(HttpContext context, ApplicationExceptionBase exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, title) = exception switch
        {
            AlreadyExistsException => (StatusCodes.Status409Conflict, "Conflict"),
            NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
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