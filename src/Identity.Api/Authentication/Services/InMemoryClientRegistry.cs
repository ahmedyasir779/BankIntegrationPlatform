using Identity.Api.Authentication.Models;

namespace Identity.Api.Authentication.Services;

public class InMemoryClientRegistry : IClientRegistry
{
    private readonly List<Client> _clients =
    [
        new Client
        {
            ClientId = "portal-client",
            ClientSecret = "SuperSecret123",
            Scopes =
            [
                "balance.read",
                "statement.read"
            ]
        },

        new Client
        {
            ClientId = "mobile-client",
            ClientSecret = "MobileSecret123",
            Scopes =
            [
                "balance.read"
            ]
        },

        new Client
        {
            ClientId = "erp-client",
            ClientSecret = "ErpSecret123",
            Scopes =
            [
                "statement.read"
            ]
        },

        new Client
        {
            ClientId = "partner-client",
            ClientSecret = "PartnerSecret123",
            Scopes =
            [
                "balance.read"
            ]
        }
    ];

    public Client? GetClient(string clientId)
    {
        return _clients.FirstOrDefault(c =>
            c.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase));
    }
}