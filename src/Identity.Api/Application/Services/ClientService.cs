using Identity.Api.Application.DTOs.Clients;
using Identity.Api.Application.Interfaces;
using Identity.Api.Domain.Entities;
using Identity.Api.Domain.Exceptions;
using Identity.Api.Infrastructure.Persistence.Repositories;

namespace Identity.Api.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _repository;

    public ClientService(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ClientResponse>> GetAllAsync()
    {
        var clients = await _repository.GetAllAsync();

        return clients.Select(MapToResponse).ToList();
    }

    public async Task<ClientResponse?> GetByIdAsync(int id)
    {
        var client = await _repository.GetByIdAsync(id);

        if (client is null)
            return null;

        return MapToResponse(client);
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest request)
    {
        var existing = await _repository.GetByClientIdAsync(request.ClientId);

        if (existing is not null)
            throw new ClientAlreadyExistsException(request.ClientId);

        var client = new Client
        {
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            Name = request.Name,
            IsActive = true,
            AllowedScopes = request.Scopes
                .Select(s => new ClientScope
                {
                    Scope = s
                })
                .ToList()
        };

        await _repository.AddAsync(client);

        return MapToResponse(client);
    }

    public async Task<ClientResponse?> UpdateAsync(int id, UpdateClientRequest request)
    {
        var client = await _repository.GetByIdAsync(id);

        if (client is null)
            throw new ClientNotFoundException(id);

        client.Name = request.Name;
        client.IsActive = request.IsActive;

        client.AllowedScopes = request.Scopes
            .Select(s => new ClientScope
            {
                Scope = s
            })
            .ToList();

        await _repository.UpdateAsync(client);

        return MapToResponse(client);
    }

    private static ClientResponse MapToResponse(Client client)
    {
        return new ClientResponse
        {
            Id = client.Id,
            ClientId = client.ClientId,
            Name = client.Name,
            IsActive = client.IsActive,
            Scopes = client.AllowedScopes
                .Select(s => s.Scope)
                .ToList()
        };
    }
}