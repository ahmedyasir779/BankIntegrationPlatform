using Identity.Api.Authentication.Models;

namespace Identity.Api.Authentication.Services;

public interface IClientRegistry
{
    Client? GetClient(string clientId);
}