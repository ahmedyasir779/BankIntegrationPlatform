using Identity.Api.Authentication.Models;

namespace Identity.Api.Authentication.Services;

public class ClientValidationService : IClientValidationService
{
    private readonly IClientRegistry _clientRegistry;

    public ClientValidationService(IClientRegistry clientRegistry)
    {
        _clientRegistry = clientRegistry;
    }

    public Client? Validate(string clientId, string clientSecret)
    {
        var client = _clientRegistry.GetClient(clientId);

        if (client is null)
            return null;

        if (client.ClientSecret != clientSecret)
            return null;

        return client;
    }
}