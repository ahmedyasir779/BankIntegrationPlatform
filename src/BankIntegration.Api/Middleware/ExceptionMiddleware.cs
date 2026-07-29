
using BankIntegration.Api.Application.Common;

namespace BankIntegration.Api.Middleware;

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

            var mapping = ExceptionMapper.Map(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = mapping.HttpStatusCode;

            var response = responseFactory.Failure<object>(
                mapping.ErrorCode,
                mapping.Description);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}