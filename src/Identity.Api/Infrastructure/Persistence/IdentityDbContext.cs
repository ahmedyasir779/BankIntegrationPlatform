using Identity.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<ClientScope> ClientScopes => Set<ClientScope>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>()
            .HasIndex(c => c.ClientId)
            .IsUnique();

        modelBuilder.Entity<Client>()
            .HasMany(c => c.AllowedScopes)
            .WithOne(s => s.Client)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed Client
        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                Id = 1,
                ClientId = "portal-client",
                ClientSecret = "SuperSecret123",
                Name = "Portal Client",
                IsActive = true
            });

        // Seed Scopes
        modelBuilder.Entity<ClientScope>().HasData(
            new ClientScope
            {
                Id = 1,
                ClientId = 1,
                Scope = "balance.read"
            },
            new ClientScope
            {
                Id = 2,
                ClientId = 1,
                Scope = "statement.read"
            });
    }
}