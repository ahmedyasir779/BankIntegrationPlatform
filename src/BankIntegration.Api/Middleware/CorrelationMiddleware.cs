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
        Guid correlationId;

        if (context.Request.Headers.TryGetValue(HeaderName, out var headerValue)
            && Guid.TryParse(headerValue, out var parsedId))
        {
            correlationId = parsedId;
        }
        else
        {
            correlationId = Guid.NewGuid();
        }

        var requestContext = new RequestContext
        {
            CorrelationId = correlationId,
            MessageId = Guid.NewGuid(),
            RequestTimeUtc = DateTime.UtcNow,
            ServiceName = context.Request.Host.Value
        };

        context.Items[HttpContextKeys.RequestContext] = requestContext;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] =
                requestContext.CorrelationId.ToString();

            return Task.CompletedTask;
        });

        await _next(context);
    }
}