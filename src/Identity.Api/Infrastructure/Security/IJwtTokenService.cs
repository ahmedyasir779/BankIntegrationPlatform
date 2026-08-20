using Identity.Api.Domain.Entities;

namespace Identity.Api.Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(Client client);
}