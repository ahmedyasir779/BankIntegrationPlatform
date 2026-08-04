using Identity.Api.Domain.Entities;

namespace Identity.Api.Authentication.Services;

public interface IClientValidationService
{
    Task<Client?> ValidateAsync(string clientId, string clientSecret);
}