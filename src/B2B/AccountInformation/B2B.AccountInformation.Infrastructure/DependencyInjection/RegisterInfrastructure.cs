using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Infrastructure.Common;
using B2B.AccountInformation.Infrastructure.External.Logic;

namespace B2B.AccountInformation.Infrastructure.DependencyInjection;

public static class RegisterInfrastructure
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LogicOptions>(
            configuration.GetSection(LogicOptions.SectionName));

        services.AddHttpContextAccessor();

        services.AddScoped<IRequestContextAccessor, RequestContextAccessor>();

        services.AddHttpClient<ILogicClient, LogicClient>(
            (provider, client) =>
            {
                var options = provider
                    .GetRequiredService<IOptions<LogicOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });

        return services;
    }
}