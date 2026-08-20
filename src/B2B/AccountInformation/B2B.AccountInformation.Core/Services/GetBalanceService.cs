using B2B.AccountInformation.Core.Interfaces;
using B2B.AccountInformation.Shared.Contracts;

namespace B2B.AccountInformation.Core.Services;

public class GetBalanceService : IGetBalanceService
{
    private readonly ILogicClient _logicClient;

    public GetBalanceService(ILogicClient logicClient)
    {
        _logicClient = logicClient;
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(
        GetBalanceRequest request)
    {
        return await _logicClient.GetBalanceAsync(request);
    }
}