using Identity.Api.Domain.Entities;
using Identity.Api.Domain.Exceptions;
using Identity.Api.Infrastructure.Persistence.Repositories;

namespace Identity.Api.Authentication.Services;

public class ClientValidationService : IClientValidationService
{
    // private readonly IClientRegistry _clientRegistry;

    private readonly IClientRepository _clientRepository;

    public ClientValidationService(
        IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Client?> ValidateAsync(string clientId, string clientSecret)
    {
        var client = await _clientRepository.GetByClientIdAsync(clientId);

        if (client is null)
            throw new InvalidClientException();

        if (client.ClientSecret != clientSecret)
            throw new InvalidClientException();

        if (!client.IsActive)
            throw new InvalidClientException();

        return client;
    }
}