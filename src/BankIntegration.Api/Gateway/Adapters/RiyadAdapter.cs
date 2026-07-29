using BankIntegration.Api.Domain.Models;

namespace BankIntegration.Api.Gateway.Adapters;

public class RiyadAdapter : IBankAdapter
{
    public string BankCode => "Riyad";
    
    public Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        var response = new BalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = 5000.15m,
            Currency = "SAR"
        };

        return Task.FromResult(response);
    }
}