using Identity.Api.Application.DTOs.Clients;

namespace Identity.Api.Application.Interfaces;

public interface IClientService
{
    Task<List<ClientResponse>> GetAllAsync();

    Task<ClientResponse?> GetByIdAsync(int id);

    Task<ClientResponse> CreateAsync(CreateClientRequest request);

    Task<ClientResponse?> UpdateAsync(int id, UpdateClientRequest request);
}