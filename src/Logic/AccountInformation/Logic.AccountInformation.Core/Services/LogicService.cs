using Logic.AccountInformation.Core.Interfaces;
// using Logic.AccountInformation.Shared.Requests;
// using Logic.AccountInformation.Shared.Responses;
using Logic.AccountInformation.Shared.Contracts;

namespace Logic.AccountInformation.Core.Services;

public class LogicService : ILogicService
{
    private readonly IBankIntegrationClient _bankIntegrationClient;

    public LogicService(
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