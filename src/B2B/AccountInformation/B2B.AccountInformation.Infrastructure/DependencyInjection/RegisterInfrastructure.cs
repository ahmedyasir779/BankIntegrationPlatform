using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Infrastructure.External.BankIntegration;

namespace B2B.AccountInformation.Infrastructure.DependencyInjection;

public static class RegisterInfrastructure
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<BankIntegrationOptions>(
            configuration.GetSection(BankIntegrationOptions.SectionName));

        services.AddHttpClient<IBankIntegrationClient, BankIntegrationClient>(
            (provider, client) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<BankIntegrationOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        options.AccessToken);
            });

        return services;
    }
}