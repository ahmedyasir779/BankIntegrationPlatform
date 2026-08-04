using Microsoft.AspNetCore.Mvc;
using Identity.Api.Authentication.Models;
using Identity.Api.Authentication.Services;
using Identity.Api.Infrastructure.Security;
using System.Linq;

namespace Identity.Api.Controllers;

[ApiController]
[Route("connect")]
public class TokenController : ControllerBase
{
    private readonly IClientValidationService _clientValidationService;
    private readonly IJwtTokenService _jwtTokenService;

    public TokenController(
        IClientValidationService clientValidationService,
        IJwtTokenService jwtTokenService)
    {
        _clientValidationService = clientValidationService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] TokenRequest request)
    {
        var client = await _clientValidationService.ValidateAsync(
                request.ClientId,
                request.ClientSecret);

        if (client is null)
        {
            return Unauthorized(new
            {
                error = "invalid_client",
                error_description = "Invalid client credentials."
            });
        }

        string accessToken = _jwtTokenService.GenerateAccessToken(client);

        var response = new TokenResponse
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = 3600,
            Scope = string.Join(
                " ",
                client.AllowedScopes.Select(s => s.Scope))
        };

        return Ok(response);
    }
}