using BankIntegrationPlatform.Domain.Models;

namespace BankIntegrationPlatform.Infrastructure.External.Adapters;

public class SNBAdapter : IBankAdapter
{
    public string BankCode => "SNB";
    public Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        var response = new BalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = 15350.25m,
            Currency = "SAR"
        };

        return Task.FromResult(response);
    }
}