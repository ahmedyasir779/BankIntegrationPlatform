using BankIntegrationPlatform.Domain.Models;
using BankIntegrationPlatform.Infrastructure.External.Http;
using BankIntegrationPlatform.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace BankIntegrationPlatform.Infrastructure.External.Adapters;

public class SNBAdapter : IBankAdapter
{
    public string BankCode => "SNB";
    private readonly IBankHttpClient _httpClient;

    private readonly BankOptions _options;

    public SNBAdapter(
        IBankHttpClient httpClient,
        IOptions<BankOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        var config = _options.Banks[BankCode];

        var url = $"{config.BaseUrl}{config.BalanceEndpoint}";

        return await _httpClient.PostAsync<BalanceRequest, BalanceResponse>(
            url,
            request);
    }
}