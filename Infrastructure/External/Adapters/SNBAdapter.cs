using BankIntegrationPlatform.Domain.Models;
using BankIntegrationPlatform.Infrastructure.External.Http;

namespace BankIntegrationPlatform.Infrastructure.External.Adapters;

public class SNBAdapter : IBankAdapter
{
    public string BankCode => "SNB";
    private readonly IBankHttpClient _httpClient;

    public SNBAdapter(IBankHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // public Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    // {
    //     var response = new BalanceResponse
    //     {
    //         AccountNumber = request.AccountNumber,
    //         Balance = 15350.25m,
    //         Currency = "SAR"
    //     };

    //     return Task.FromResult(response);
    // }

    public async Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        return await _httpClient.PostAsync<BalanceRequest, BalanceResponse>(
            "/balance",
            request);
    }
}