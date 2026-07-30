using Identity.Api.Domain.Entities;

namespace Identity.Api.Infrastructure.Persistence.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByClientIdAsync(string clientId);
}