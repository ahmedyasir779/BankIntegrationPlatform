using Identity.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IdentityDbContext _context;

    public ClientRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByClientIdAsync(string clientId)
    {
        return await _context.Clients
            .Include(c => c.AllowedScopes)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }
}