namespace Identity.Api.Authentication.Models;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public int ExpiresIn { get; set; }

    public string Scope { get; set; } = string.Empty;
}