using BankIntegration.Api.Domain.Models;

namespace BankIntegration.Api.Gateway.Adapters;

public class AlRajhiAdapter : IBankAdapter
{
    public string BankCode => "AlRajhi";
    public Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        var response = new BalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = 1200.95m,
            Currency = "USD"
        };

        return Task.FromResult(response);
    }
}