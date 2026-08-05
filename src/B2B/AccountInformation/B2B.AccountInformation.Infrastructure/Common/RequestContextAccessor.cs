using Microsoft.AspNetCore.Http;
using B2B.AccountInformation.Core.Interfaces;

namespace B2B.AccountInformation.Infrastructure.Common;

public class RequestContextAccessor : IRequestContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? AccessToken =>
        _httpContextAccessor.HttpContext?
            .Request
            .Headers["Authorization"]
            .FirstOrDefault();
}