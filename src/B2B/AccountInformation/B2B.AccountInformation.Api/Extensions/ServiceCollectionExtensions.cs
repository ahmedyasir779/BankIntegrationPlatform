using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Core.Services;
using B2B.AccountInformation.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace B2B.AccountInformation.Api.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IGetBalanceService, GetBalanceService>();

        services.AddInfrastructure(configuration);

        return services;
    }
}