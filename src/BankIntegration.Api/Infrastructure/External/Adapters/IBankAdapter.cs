using BankIntegration.Api.Domain.Models;

namespace BankIntegration.Api.Infrastructure.External.Adapters;

public interface IBankAdapter
{
    string BankCode { get; }
    
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request);
}