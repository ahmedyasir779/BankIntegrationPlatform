using Microsoft.AspNetCore.Http;

namespace BankIntegration.Api.Common;

public class RequestContextAccessor : IRequestContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public RequestContext Context
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
                throw new InvalidOperationException("No active HttpContext.");

            if (httpContext.Items.TryGetValue(
                    HttpContextKeys.RequestContext,
                    out var value)
                && value is RequestContext requestContext)
            {
                return requestContext;
            }

            throw new InvalidOperationException("RequestContext not found.");
        }

        set
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
                throw new InvalidOperationException("No active HttpContext.");

            httpContext.Items[HttpContextKeys.RequestContext] = value;
        }
    }
}