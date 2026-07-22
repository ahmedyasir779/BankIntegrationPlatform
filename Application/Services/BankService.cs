using BankIntegrationPlatform.Application.Interfaces;
using BankIntegrationPlatform.Domain.Models;

namespace BankIntegrationPlatform.Application.Services;

public class BankService : IBankService
{
    public BalanceResponse GetBalance(BalanceRequest request)
    {
        return new BalanceResponse
        {
            AccountNumber = request.AccountNumber,
            Balance = 15350.25m,
            Currency = "SAR"
        };
    }
}