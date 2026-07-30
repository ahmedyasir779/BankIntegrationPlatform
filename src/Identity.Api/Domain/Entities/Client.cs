using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ClientSecret { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<ClientScope> AllowedScopes { get; set; }
        = new List<ClientScope>();
}