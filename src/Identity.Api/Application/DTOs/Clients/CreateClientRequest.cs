namespace Identity.Api.Application.DTOs.Clients;

public class CreateClientRequest
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<string> Scopes { get; set; } = new();
}