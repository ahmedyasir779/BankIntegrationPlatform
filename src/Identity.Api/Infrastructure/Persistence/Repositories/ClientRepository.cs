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

    public async Task<List<Client>> GetAllAsync()
    {
        return await _context.Clients
            .Include(c => c.AllowedScopes)
            .ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients
            .Include(c => c.AllowedScopes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Client?> GetByClientIdAsync(string clientId)
    {
        return await _context.Clients
            .Include(c => c.AllowedScopes)
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task AddAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }
}