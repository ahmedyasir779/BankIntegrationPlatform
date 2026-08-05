using Microsoft.AspNetCore.Http;
using Logic.AccountInformation.Core.Interfaces;

namespace Logic.AccountInformation.Infrastructure.Common;

public class RequestContextAccessor : IRequestContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextAccessor(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? AccessToken
    {
        get
        {
            var header = _httpContextAccessor
                .HttpContext?
                .Request
                .Headers["Authorization"]
                .FirstOrDefault();

            return header;
        }
    }
}