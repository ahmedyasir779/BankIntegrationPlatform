using BankIntegration.Api.Domain.Models;

namespace BankIntegration.Api.Gateway.Adapters;

public class MockBankAdapter : IBankAdapter
{
    public string BankCode => "Mock";
    
    public Task<BalanceResponse> GetBalanceAsync(BalanceRequest request)
    {
        var response = new BalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = 5.00m,
            Currency = "SAR"
        };

        return Task.FromResult(response);
    }
}