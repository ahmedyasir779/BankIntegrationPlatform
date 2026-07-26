using BankIntegrationPlatform.Domain.Models;

namespace BankIntegrationPlatform.Infrastructure.External.Adapters;

public interface IBankAdapter
{
    string BankCode { get; }
    
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request);
}