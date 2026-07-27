using System.Text.Json;

namespace BankIntegrationPlatform.Middleware;

using BankIntegrationPlatform.Domain.Messages;
using BankIntegrationPlatform.Common;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
    HttpContext context,
    IRequestContextAccessor requestContext)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(
                context,
                exception,
                requestContext);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, IRequestContextAccessor requestContext)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var currentContext = requestContext.Context;

        var response = new ApiResponse<object>
        {

            Header = new ResponseHeader
            {

                CorrelationId = currentContext.CorrelationId,
                MessageId = currentContext.MessageId,
                TimestampUtc = currentContext.RequestTimeUtc,

                Status = new ResponseStatus
                {
                    StatusType = "Error",
                    StatusCode = "500",
                    StatusDescription = exception.Message
                }
            },

            Data = null
        };

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);

    }
}