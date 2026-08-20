namespace BankIntegration.Api.Middleware;

using BankIntegration.Api.Common;
public class CorrelationMiddleware
{
    private const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    // private readonly IRequestContextAccessor _requestContextAccessor;

    public CorrelationMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(
        HttpContext context,
        IRequestContextAccessor requestContextAccessor)
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
            ServiceName = "BankIntegration.Api",
            ApiVersion = "v1",
            RequestPath = context.Request.Path,
            HttpMethod = context.Request.Method
        };

        requestContextAccessor.Context = requestContext;
        // context.Items[HttpContextKeys.RequestContext] = requestContext;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] =
                requestContext.CorrelationId.ToString();

            return Task.CompletedTask;
        });

        await _next(context);
    }
}