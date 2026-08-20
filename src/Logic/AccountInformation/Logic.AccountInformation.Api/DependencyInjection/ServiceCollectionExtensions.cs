using Logic.AccountInformation.Core.Interfaces;
using Logic.AccountInformation.Core.Services;
using Logic.AccountInformation.Infrastructure.DependencyInjection;

namespace Logic.AccountInformation.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ILogicService, LogicService>();

        services.AddInfrastructure(configuration);

        return services;
    }
}