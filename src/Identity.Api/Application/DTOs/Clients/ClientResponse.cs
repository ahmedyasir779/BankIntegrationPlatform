namespace Identity.Api.Application.DTOs.Clients;

public class ClientResponse
{
    public int Id { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<string> Scopes { get; set; } = new();
}