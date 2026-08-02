using Identity.Api.Domain.Entities;

namespace Identity.Api.Infrastructure.Persistence.Repositories;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();

    Task<Client?> GetByIdAsync(int id);

    Task<Client?> GetByClientIdAsync(string clientId);

    Task AddAsync(Client client);

    Task UpdateAsync(Client client);
}