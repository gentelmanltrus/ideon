using System.Net;
using System.Text.Json;
using Ideon.API.Exceptions;

namespace Ideon.API.Middleware;
// Middleware for handling exceptions that occur during the processing of HTTP requests. It catches exceptions, logs them, and returns appropriate HTTP responses to the client.
public class ExceptionHandlingMiddleware
{
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
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An error occurred while processing the request."
            );

            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                IdeaNotFoundException => (int)HttpStatusCode.NotFound,

                CollaborationRequestConflictException =>
                    (int)HttpStatusCode.Conflict,

                ArgumentOutOfRangeException =>
                    (int)HttpStatusCode.BadRequest,

                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var message = statusCode == 500
                ? "An unexpected server error occurred."
                : exception.Message;

            var response = new
            {
                statusCode,
                message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}