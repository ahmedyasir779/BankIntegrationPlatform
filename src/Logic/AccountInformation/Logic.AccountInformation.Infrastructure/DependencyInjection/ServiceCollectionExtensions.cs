using Logic.AccountInformation.Core.Interfaces;
using Logic.AccountInformation.Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Logic.AccountInformation.Infrastructure.Common;
namespace Logic.AccountInformation.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IRequestContextAccessor, RequestContextAccessor>();
        
        services.AddHttpClient<IBankIntegrationClient, BankIntegrationClient>(
            client =>
            {
                client.BaseAddress = new Uri(
                    configuration["BankIntegration:BaseUrl"]!);
            });
        

        return services;
    }
}