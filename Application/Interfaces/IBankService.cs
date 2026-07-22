using BankIntegrationPlatform.Domain.Models;

namespace BankIntegrationPlatform.Application.Interfaces;

public interface IBankService
{
    BalanceResponse GetBalance(BalanceRequest request);
}