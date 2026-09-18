using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Presentation.Models;

namespace InfigoHR.Presentation.Middleware;

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
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ErrorResponseModel { Message = ex.Message });
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationErrorResponseModel
            {
                Errors = ex.Errors.Select(x => x.ErrorMessage)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled error while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                new ErrorResponseModel
                {
                    Message = "An unexpected error occurred."
                });
        }
    }
}
