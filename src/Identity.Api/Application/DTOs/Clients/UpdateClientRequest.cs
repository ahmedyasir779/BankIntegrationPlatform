namespace Identity.Api.Application.DTOs.Clients;

public class UpdateClientRequest
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<string> Scopes { get; set; } = new();
}