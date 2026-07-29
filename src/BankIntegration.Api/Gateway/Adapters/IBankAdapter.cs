using BankIntegration.Api.Domain.Models;

namespace BankIntegration.Api.Gateway.Adapters;

public interface IBankAdapter
{
    string BankCode { get; }
    
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request);
}