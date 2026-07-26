namespace BankIntegrationPlatform.Middleware;

using BankIntegrationPlatform.Common;
public class CorrelationMiddleware
{
    private const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the client already sent a Correlation ID.
        string correlationId =
            context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
                ? headerValue.ToString()
                : Guid.NewGuid().ToString();

        // Store it for the rest of this request.
        context.Items[HttpContextKeys.CorrelationId] = correlationId;

        // we use OnStarting to make him run this code before the header is sent
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Continue to the next middleware.
        await _next(context);

        // Include it in the response sent back to the client.
        // context.Response.Headers[HeaderName] = correlationId;
    }
}