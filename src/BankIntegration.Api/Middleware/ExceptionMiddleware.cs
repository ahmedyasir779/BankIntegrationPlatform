
using BankIntegrationPlatform.Application.Common;

namespace BankIntegrationPlatform.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IApiResponseFactory responseFactory,
        ILogger<ExceptionMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred.");

            var response = exception switch
            {
                BankAdapterNotFoundException =>
                    responseFactory.Failure<object>(
                        "404",
                        exception.Message),

                _ =>
                    responseFactory.Failure<object>(
                        "500",
                        "Internal server error.")
            };

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                BankAdapterNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}