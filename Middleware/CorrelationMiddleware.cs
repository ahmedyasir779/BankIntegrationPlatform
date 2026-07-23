namespace BankIntegrationPlatform.Middleware;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // We'll implement this tomorrow.

        await _next(context);
    }
}