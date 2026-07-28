using Identity.Api.Authentication.Models;

namespace Identity.Api.Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(Client client);
}