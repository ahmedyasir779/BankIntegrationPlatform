using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Shared.Contracts;

namespace B2B.AccountInformation.Core.Services;

public class GetBalanceService : IGetBalanceService
{
    private readonly IBankIntegrationClient _bankIntegrationClient;

    public GetBalanceService(
        IBankIntegrationClient bankIntegrationClient)
    {
        _bankIntegrationClient = bankIntegrationClient;
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(
        GetBalanceRequest request)
    {
        return await _bankIntegrationClient.GetBalanceAsync(request);
    }
}