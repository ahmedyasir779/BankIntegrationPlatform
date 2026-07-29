using Identity.Api.Authentication.Models;

namespace Identity.Api.Authentication.Services;

public interface IClientValidationService
{
    Client? Validate(string clientId, string clientSecret);
}