using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Api.Infrastructure.Security;
using Identity.Api.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Infrastructure.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public string GenerateAccessToken(Client client)
    {

        var claims = new List<Claim>
        {
            // The Subject of the token.
            new(JwtRegisteredClaimNames.Sub, client.ClientId),
            // Unique JWT identifier.
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // A custom claim that APIs can read directly.
            new("client_id", client.ClientId)
        };

        foreach (var scope in client.Scopes)
        {
            // Adds one claim for each permitted scope.
            claims.Add(new Claim("scope", scope));
        }

        // Creates the cryptographic signing key from your configured secret.
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        // Specifies how the token will be signed.
        // We're using: HMAC SHA-256
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Builds the complete JWT by combining:
        // Issuer, Audience, Claims, Expiration, Signature
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials);

        // Serialises the JWT object into the compact string that clients send in the Authorization header
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}