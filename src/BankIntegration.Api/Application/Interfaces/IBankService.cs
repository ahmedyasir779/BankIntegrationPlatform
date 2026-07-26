using BankIntegrationPlatform.Domain.Models;

namespace BankIntegrationPlatform.Application.Interfaces;

public interface IBankService
{
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request);
}