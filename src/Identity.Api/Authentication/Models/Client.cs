namespace Identity.Api.Authentication.Models;
public class Client
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public List<string> Scopes { get; set; } = new();
}