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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        Guid correlationId = Guid.Empty;

        if (context.Items.TryGetValue(HttpContextKeys.CorrelationId, out var value))
        {
            Guid.TryParse(value?.ToString(), out correlationId);
        }

        var response = new ApiResponse<object>
        {

            Header = new ResponseHeader
            {

                MessageId = Guid.NewGuid(),
                CorrelationId = correlationId,
                TimestampUtc = DateTime.UtcNow,

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