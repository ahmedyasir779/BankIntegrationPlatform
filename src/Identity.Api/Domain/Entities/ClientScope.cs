using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Domain.Entities;

public class ClientScope
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Scope { get; set; } = string.Empty;

    public Client Client { get; set; } = null!;
}